using System.Diagnostics;
using System.Text;


public static class UKProcess
{
	public static string Run(string workingDirectory, string command, string arguments) {
		ProcessStartInfo psi = new ProcessStartInfo();
		
		psi.WorkingDirectory = workingDirectory;
		psi.FileName = command;
		psi.Arguments = arguments;
		psi.UseShellExecute = false;
		psi.RedirectStandardOutput = true;
		psi.CreateNoWindow = true;
		
		var proc = Process.Start(psi);

		var sb = new StringBuilder();

		while (!proc.StandardOutput.EndOfStream) {
			sb.Append(proc.StandardOutput.ReadLine());
		}

		return sb.ToString();
	}
}

