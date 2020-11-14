using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable StringLiteralTypo

namespace JarLauncher {

internal static class Program {
    
    private static void Main(string[] args) {
        if (args.Length < 1) {
            MessageBox.Show("没有提供要打开的文件", "JarLauncher",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        var launcher = new Launcher();
        launcher.checkVmOptionsFile();
        string vmOptions = launcher.readVmOptions();
        launcher.launch(vmOptions, args[0]);
    }
}

internal class Launcher {

    public void launch(string vmOptions, string jarPath) {
        //启动命令行
        var process = new Process {
            StartInfo = {
                //设置要启动的应用程序
                FileName = "cmd.exe",
                //是否使用操作系统shell启动
                UseShellExecute = false,
                //接受来自调用程序的输入信息
                RedirectStandardInput = true,
                //输出信息
                RedirectStandardOutput = true,
                //输出错误
                RedirectStandardError = true,
                //不显示程序窗口
                CreateNoWindow = true
            }
        };
        //启动程序
        process.Start();
        process.StandardInput.WriteLine($"javaw {vmOptions} -jar \"{jarPath}\"");
        process.StandardInput.WriteLine("exit");   //需要有这句，不然程序会挂机
        //等待程序执行完退出进程
        process.WaitForExit();
        process.Close();
    }

    public string readVmOptions() {
        var fileStream = new FileStream(vmOptionsPath, FileMode.Open,
            FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        string content = streamReader.ReadToEnd();
        streamReader.Close();
        fileStream.Close();
        return content;
    }

    public void checkVmOptionsFile() {
        if(File.Exists(vmOptionsPath)) return;
        var fileStream = new FileStream(vmOptionsPath, FileMode.Create,
            FileAccess.ReadWrite);
        var streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(defaultVmOptions);
        streamWriter.Close();
        fileStream.Close();
    }

    private static readonly string
        vmOptionsPath = Application.StartupPath + "\\vm_options.txt";
    private const string defaultVmOptions = "-Dfile.encoding=UTF-8";
}

}