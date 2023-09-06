namespace Framework.HL7.Models
{
    public abstract class MessageElement
    {
        protected string _value = string.Empty;
        
        public  string Value 
        { 
            get { return _value; }
            set { _value = value; ProcessValue(); }
        }

        public Encoding Encoding { get; protected set; }

        protected abstract void ProcessValue();
    }
}
