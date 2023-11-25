// Tujuan: Menulis log ke console

// membuat namespace
namespace es_manage.api.Utilities {
    public static class Logger {
        // Jenis jenis log
        public enum LogType {
            Info,
            Warning,
            Error
        }

        public static void WriteToConsole(LogType type, string message) {
            // Menentukan warna tulisan berdasarkan jenis log
            switch (type) {
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            // Menulis log ke console
            // Format: [yyyy-MM-dd HH:mm:ss] {type}: {message}
            // Contoh: [2023-07-10 10:10:10] INFO: Ini adalah contoh log
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {type.ToString().ToUpper()}: {message}");
            Console.ResetColor();
        }
    }
}