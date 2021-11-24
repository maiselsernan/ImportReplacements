namespace ImportReplacement.Models
{
   public enum RequestTypes
    {
        Undefined = 0,
        Maintain = 1,
        Replace = 2,
        Installation = 3,
        DuplicateMeter = 4,
        Ignore = 5,
        MaintainRequiredChannel = 6,
        ReplaceRequiredChannel = 7,
        InstallationRequiredChannel = 8
    }

   public static class RequestTypesExtensions
   {
       public static string Name(this RequestTypes requestType)
       {
           switch (requestType.ToString())
           {
                case null:
                case "":
                   return string.Empty;
                case "MaintainRequiredChannel":
                case "ReplaceRequiredChannel":
                case "InstallationRequiredChannel":
                    return "Required Channels";
                default:
                    return requestType.ToString();
           }
       }
   }
}
