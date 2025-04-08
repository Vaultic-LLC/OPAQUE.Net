namespace OPAQUE.Net.Types.Parameters
{
    public class KSFConfig
    {
        public KSFConfigType Type { get; private set; } = KSFConfigType.MemoryConstrained;

        public string Iterations { get; private set; } = string.Empty;
        public string Memory { get; private set; } = string.Empty;
        public string Parallelism { get; private set; } = string.Empty;

        public string TypeString 
        { 
            get
            {
                switch (Type)
                {
                    case KSFConfigType.RfcDraftRecommended:
                        return "rfcDraftRecommended";
                    case KSFConfigType.Custom:
                        return "custom";
                    case KSFConfigType.MemoryConstrained:
                    default:
                        return "memoryConstrained";
                }
            }
        }

        protected KSFConfig() { }

        public static KSFConfig Create(KSFConfigType type, int? iterations = null, int? memory = null, int? parallelism = null)
        {
            if (type == KSFConfigType.Custom)
            {
                if (iterations == null || memory == null || parallelism == null)
                {
                    throw new ArgumentException();
                }

                return new KSFConfig()
                {
                    Type = type,
                    Iterations = iterations.Value.ToString(),
                    Memory = memory.Value.ToString(),
                    Parallelism = parallelism.Value.ToString()
                };
            }

            return new KSFConfig()
            {
                Type = type
            };
        }
    }

    public enum KSFConfigType
    {
        RfcDraftRecommended,
        MemoryConstrained,
        Custom
    }
}
