using System;
using System.Collections;
using System.Text;

namespace WindowsFormsApplication1
{
    #region Dummy
    public class MonoBehavior
    {
        public void SendMessage(string methodName) {} 
        public void StartCoroutine(IEnumerator method) {}
    }
    
    public class YieldInstruction {}
    
    public class WaitForSeconds : YieldInstruction
    {
        public WaitForSeconds(float waitSeconds) : base() {}
    }
    #endregion
    
    public class Effect
    {
        private string beginMessage;
        private string endMessage;
        
        private bool isEffectContinued;
        
        public Effect(string beginMessage, string endMessage)
        {
            this.beginMessage = beginMessage;
            this.endMessage = endMessage;
        }
        
        public void Begin(MonoBehavior target, float retainTick)
        {
            if (isEffectContinued == false)
            {
                isEffectContinued = true;
                
                target.StartCoroutine(RetainCheck(target, retainTick));
            }
        }
        
        private IEnumerator RetainCheck(MonoBehavior target, float retainTick)
        {
            target.SendMessage(this.beginMessage);
        
            isEffectContinued = false;
        
            yield return new WaitForSeconds(retainTick);
            
            if (isEffectContinued == true)
            {
            }
        }
    }
}
