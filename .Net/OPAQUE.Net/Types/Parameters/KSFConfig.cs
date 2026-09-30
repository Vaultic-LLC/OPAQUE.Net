namespace OPAQUE.Net.Types.Parameters
{
    public class KSFConfig
    {
        public const int MinimumMemoryKiB = 64 * 1024;
        public const int MaximumCustomMemoryKiB = 1024 * 1024;
        public const int MinimumIterations = 1;
        public const int MaximumIterations = 10;
        public const int MinimumParallelism = 1;
        public const int MaximumParallelism = 16;
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
                    throw new ArgumentException("Custom KSF configuration requires iterations, memory, and parallelism.");
                }

                ArgumentOutOfRangeException.ThrowIfLessThan(iterations.Value, MinimumIterations, nameof(iterations));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(iterations.Value, MaximumIterations, nameof(iterations));
                ArgumentOutOfRangeException.ThrowIfLessThan(memory.Value, MinimumMemoryKiB, nameof(memory));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(memory.Value, MaximumCustomMemoryKiB, nameof(memory));
                ArgumentOutOfRangeException.ThrowIfLessThan(parallelism.Value, MinimumParallelism, nameof(parallelism));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(parallelism.Value, MaximumParallelism, nameof(parallelism));

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
