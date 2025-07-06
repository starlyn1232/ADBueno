namespace ADBueno.Classes
{
    public class DetectedAdb
    {
        // Attributes
        public string SerialNumber { get; set; }
        public ADB.ADBModes Mode { get; set; }
        public bool IsReady { get; set; }

        // Constructor
        public DetectedAdb(string sn, ADB.ADBModes mode, bool isReady)
        {
            this.SerialNumber = sn;
            this.Mode = mode;
            IsReady = isReady;
        }
    }
}
