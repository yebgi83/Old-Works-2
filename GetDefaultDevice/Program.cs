using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using NAudio;
using NAudio.Mixer;
using NAudio.CoreAudioApi;

namespace GetDefaultDevice
{
    class Program
    {
        static MixerInterop.MIXERLINE GetDestinationLine(IntPtr mixerHandle, int destinationIndex)
        {
            MixerInterop.MIXERLINE mixerLine = new MixerInterop.MIXERLINE()
            {
                cbStruct = Marshal.SizeOf(typeof(MixerInterop.MIXERLINE)),
                dwDestination = destinationIndex
            };
              
            MmException.Try
            (
                MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine , MixerFlags.GetLineInfoOfDestination),
                "mixerGetLineInfo"
            );
            
            return mixerLine;
        }
        
        static MixerInterop.MIXERLINE GetSourceLine(IntPtr mixerHandle, int destinationIndex, int sourceIndex)
        {
            MixerInterop.MIXERLINE mixerLine = new MixerInterop.MIXERLINE()
            {
                cbStruct = Marshal.SizeOf(typeof(MixerInterop.MIXERLINE)),
                dwDestination = destinationIndex,
                dwSource = sourceIndex
            };
              
            MmException.Try
            (
                MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine , MixerFlags.GetLineInfoOfSource),
                "mixerGetLineInfo"
            );
            
            return mixerLine;
        }
        
        static MixerInterop.MIXERLINECONTROLS GetLineControls(IntPtr mixerHandle, MixerInterop.MIXERLINE mixerLine)
        {
            MixerInterop.MIXERLINECONTROLS mixerLineControls = new MixerInterop.MIXERLINECONTROLS()
            {
                cbStruct = Marshal.SizeOf(typeof(MixerInterop.MIXERLINECONTROLS)),
                cControls = mixerLine.cControls,
                dwLineID = mixerLine.dwLineID,
                cbmxctrl = Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROL)),
                pamxctrl = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROL)) * mixerLine.cControls)
            };
            
            MmException.Try
            (
                MixerInterop.mixerGetLineControls(mixerHandle, ref mixerLineControls, MixerFlags.All),
                "mixerGetLineControls"
            );        
            
            return mixerLineControls;
        }
        
        static MixerInterop.MIXERCONTROL GetControl(IntPtr mixerHandle, MixerInterop.MIXERLINECONTROLS mixerLineControls, int controlIndex)
        {
            MixerInterop.MIXERCONTROL mixerControl = (MixerInterop.MIXERCONTROL)Marshal.PtrToStructure
            (
                (IntPtr)(mixerLineControls.pamxctrl.ToInt64() + Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROL)) * controlIndex),
                typeof(MixerInterop.MIXERCONTROL)
            );
            
            return mixerControl;
        }
        
        static int GetMuxIndex(IntPtr mixerHandle, MixerInterop.MIXERCONTROL muxControl)
        {
            int mixerControlDetailsSize = Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROLDETAILS));
            int valueSize = Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROLDETAILS_BOOLEAN));
            
            MixerInterop.MIXERCONTROLDETAILS mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS()
            {
                cbStruct = mixerControlDetailsSize,
                cChannels = 1,
                dwControlID = muxControl.dwControlID,
                hwndOwner = (IntPtr)muxControl.cMultipleItems,
                cbDetails = valueSize,
                paDetails = Marshal.AllocCoTaskMem(valueSize * (int)muxControl.cMultipleItems)
            };
        
            try
            {
                MmException.Try
                (
                    MixerInterop.mixerGetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value),
                    "mixerGetControlDetails"
                );

                for (int muxIndex = 0; muxIndex < muxControl.cMultipleItems; muxIndex++)
                {
                    MixerInterop.MIXERCONTROLDETAILS_BOOLEAN value = (MixerInterop.MIXERCONTROLDETAILS_BOOLEAN)Marshal.PtrToStructure
                    (
                        (IntPtr)(mixerControlDetails.paDetails.ToInt64() + Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROLDETAILS_BOOLEAN)) * muxIndex), 
                        typeof(MixerInterop.MIXERCONTROLDETAILS_BOOLEAN)
                    );
                    
                    if (value.fValue == 1)
                    {
                        return (int)(muxControl.cMultipleItems - (1 + muxIndex));
                    }
                }
                
                return -1;
            }
            finally
            {
                Marshal.FreeCoTaskMem(mixerControlDetails.paDetails);
            }
        }
        
        static int GetVolumeValue(IntPtr mixerHandle, MixerInterop.MIXERCONTROL volumeControl)
        {
            int mixerControlDetailsSize = Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROLDETAILS));
            int valueSize = Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED));
            
            MixerInterop.MIXERCONTROLDETAILS mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS()
            {
                cbStruct = mixerControlDetailsSize,
                cChannels = 0,
                dwControlID = volumeControl.dwControlID,
                hwndOwner = IntPtr.Zero,
                cbDetails = valueSize,
                paDetails = Marshal.AllocCoTaskMem(valueSize)
            };
        
            try
            {
                MmException.Try
                (
                    MixerInterop.mixerGetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value),
                    "mixerGetControlDetails"
                );

                MixerInterop.MIXERCONTROLDETAILS_UNSIGNED value = (MixerInterop.MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure
                (
                    (IntPtr)(mixerControlDetails.paDetails.ToInt64() + Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED))), 
                    typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED)
                );
                
                return (int)value.dwValue;
            }
            finally
            {
                Marshal.FreeCoTaskMem(mixerControlDetails.paDetails);
            }
        }
       
        // OS 버전에 맞는 녹음 컨트롤을 가져온다.
        static private bool GetRecordVolumeControl(ref object volumeControl, ref string errMsg)
        {
            try
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    MixerControl muxControl = null;
                    
                    foreach(Mixer mixer in Mixer.Mixers)
                    {
                        foreach(MixerLine mixerDestination in mixer.Destinations)
                        {
                            // 녹음 믹서인지 체크한다.
                            if (mixerDestination.ComponentType != MixerLineComponentType.DestinationWaveIn)
                            {
                                continue;
                            }
                            
                            // 녹음 믹서라면 녹음 장치와 연결된 Destination 장치에서 Mux, Volume 장치를 찾는다.
                            // 녹음 컨트롤에서 특정 장치를 선택할 수 있다면 Mux 장치가 감지될 것이고 
                            // 만약 선택은 할 수 없지만, 마스터 볼륨으로 제공이 된다면 Volume 장치를 감지할 것이다.
                            foreach(MixerControl mixerControl in mixerDestination.Controls)
                            {
                                if (mixerControl.ControlType == MixerControlType.Mux)
                                {
                                    muxControl = mixerControl;
                                }
                            
                                if (mixerControl.ControlType == MixerControlType.Volume)
                                {
                                    volumeControl = mixerControl;
                                }
                                
                                if (muxControl != null && volumeControl != null)
                                {
                                    break;
                                }
                            }
                            
                            // Mux 장치를 찾았을 경우, 선택된 녹음 장치에서 Volume 장치를 찾는다.
                            if (muxControl != null)
                            {
                                MixerLine sourceSelected = mixerDestination.GetSource((muxControl as MuxMixerControl).GetSelectedIndex());
                                
                                foreach(MixerControl mixerControl in sourceSelected.Controls)
                                {
                                    if (mixerControl.ControlType == MixerControlType.Volume)
                                    {
                                        volumeControl = mixerControl;
                                        break;
                                    }
                                }
                            }
                            
                            // Volume 장치를 찾았으면 True를 리턴한다.
                            if (volumeControl != null)
                            {
                                return true;
                            }                            
                        }
                    }
                    
                    // 실패하면 False를 리턴한다.
                    return false;
                }
                else
                {
                    MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
                    MMDevice device = deviceEnum.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Capture, NAudio.CoreAudioApi.Role.Multimedia);
                    
                    volumeControl = device.AudioEndpointVolume;
                    return true;
                }
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }
        
        static void Main(string[] args)
        {
            object control = null;
            string errMsg = null;
            
            
            
            //GetRecordVolumeControl(ref control, ref errMsg);
        }
        
        static void OldMethod()
        {
            MixerInterop.MIXERCONTROL? muxControl = null;
            MixerInterop.MIXERCONTROL? volumeControl = null;
            
            IntPtr mixerHandle;
            
            for (int mxId = 0; mxId  < Mixer.NumberOfDevices; mxId++)
            {
                MmException.Try
                (
                    MixerInterop.mixerOpen(out mixerHandle, mxId, IntPtr.Zero, IntPtr.Zero, MixerFlags.Mixer),
                    "mixerOpen"
                );
                
                try
                {
                    MixerInterop.MIXERCAPS mixerCaps = new MixerInterop.MIXERCAPS();
                    
                    MmException.Try
                    (
                        MixerInterop.mixerGetDevCaps((IntPtr)mxId, ref mixerCaps, Marshal.SizeOf(mixerCaps)),
                        "mixerGetDevCaps"
                    );
                    
                    for (int destLineIndex = 0; destLineIndex < mixerCaps.cDestinations; destLineIndex++)
                    {
                        MixerInterop.MIXERLINE destMixerLine = GetDestinationLine(mixerHandle, destLineIndex);
                          
                        if (destMixerLine.dwComponentType != MixerLineComponentType.DestinationWaveIn)
                        {
                            continue;
                        }
                        
                        MixerInterop.MIXERLINECONTROLS destMixerLineControls = GetLineControls(mixerHandle, destMixerLine);
                               
                        for (int destControlIndex = 0; destControlIndex < destMixerLine.cControls; destControlIndex++)
                        {
                            MixerInterop.MIXERCONTROL destMixerControl = GetControl(mixerHandle, destMixerLineControls, destControlIndex);
                            
                            if (destMixerControl.dwControlType == MixerControlType.Mux)
                            {
                                muxControl = destMixerControl;
                            }
                            else if (destMixerControl.dwControlType == MixerControlType.Volume)
                            {
                                volumeControl = destMixerControl;
                            }
                            
                            if (muxControl == null || volumeControl == null)
                            {
                                continue;
                            }
                        }
                        
                        if (muxControl != null)
                        {
                            int muxIndex = GetMuxIndex(mixerHandle, muxControl.Value);
                            
                            if (muxIndex >= 0)
                            {
                                MixerInterop.MIXERLINE srcMixerLine = GetSourceLine(mixerHandle, destLineIndex, muxIndex);
                                MixerInterop.MIXERLINECONTROLS srcMixerControls = GetLineControls(mixerHandle, srcMixerLine);
                                
                                for (int srcControlIndex = 0; srcControlIndex < srcMixerLine.cControls; srcControlIndex++)
                                {
                                    MixerInterop.MIXERCONTROL srcMixerControl = GetControl(mixerHandle, srcMixerControls, srcControlIndex);
                                
                                    if (srcMixerControl.dwControlType == MixerControlType.Volume)
                                    {
                                        volumeControl = srcMixerControl;
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    MmException.Try
                    (
                        MixerInterop.mixerClose(mixerHandle),
                        "mixerClose"
                    );
                }
            }
            
            Console.WriteLine(muxControl);
            Console.WriteLine(volumeControl);
        }
    }
}
