using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Storage.Streams;

namespace OneSound.DTO;

public record class SessionDto(string Aumid, string DisplayName, string Base64EncodedIcon);

public class DtoHelper
{
    public static async Task<SessionDto?> GetFromAumid(string aumid)
    {
        SessionDto dto;
        string displayName;
        string base64EncodedIcon;

        // check if program is a UWP app
        try
        {
            AppInfo appInfo = AppInfo.GetFromAppUserModelId(aumid);
            RandomAccessStreamReference streamReference = appInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(64, 64));
            Windows.Storage.Streams.Buffer buffer = new(64 * 64);

            var randomAccessStream = await streamReference.OpenReadAsync();
            await randomAccessStream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);
            
            displayName = appInfo.Package.DisplayName;
            base64EncodedIcon = Convert.ToBase64String(buffer.ToArray());
        } 
        // use for Win32 apps
        catch
        {
            // Process is not always alive, for example Chrome might be registered
            // but when this is requested, Chrome is not open so icon is not extractable. 
            // Could store the executable path itself somewhere without getting the process 
            // but its not worth it, this doesn't really change app functionality
            Process[] processes = Process.GetProcessesByName(aumid);
            
            displayName = aumid;
            base64EncodedIcon = "";
            
            if (processes.Length != 0) {
                Process process = processes[0];
                string? fileName = process.MainModule?.FileName;
                MemoryStream stream = new();

                Icon? icon = Icon.ExtractAssociatedIcon(fileName!);
                Bitmap bmp = icon!.ToBitmap();
                bmp.Save(stream, ImageFormat.Png);
                
                base64EncodedIcon = Convert.ToBase64String(stream.GetBuffer());
            };
        }

        dto = new SessionDto(aumid, displayName, base64EncodedIcon);
        
        return dto;
    }
}