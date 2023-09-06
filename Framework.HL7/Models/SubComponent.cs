namespace Framework.HL7.Models
{
    public class SubComponent : MessageElement
    {
        public SubComponent(string val)
        {
            this.Value = val;
        }

        protected override void ProcessValue()
        {
            
        }
    }
}
