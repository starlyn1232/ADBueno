namespace ADBueno.Classes
{
    public class DetectedAdb
    {
        // Attributes
        public string SerialNumber { get; set; }
        public string Mode { get; set; }

        // Constructor
        public DetectedAdb(string sn, string mode)
        {
            this.SerialNumber = sn;
            this.Mode = mode;
        }
    }
}
