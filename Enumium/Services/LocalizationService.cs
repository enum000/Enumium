namespace Enumium.Services
{
    public static class LocalizationService
    {
        public static string CurrentLanguage { get; set; } = "en";

        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            // ═══════════════════ ENGLISH ═══════════════════
            ["en"] = new Dictionary<string, string>
            {
                // Navigation
                ["nav_dashboard"] = "Dashboard",
                ["nav_performance"] = "Performance",
                ["nav_cleaner"] = "Cleaner",
                ["nav_startup"] = "Startup",
                ["nav_network"] = "Network",
                ["nav_privacy"] = "Privacy",
                ["nav_tools"] = "Tools",
                ["nav_reports"] = "Reports",
                ["nav_about"] = "About",

                // Dashboard
                ["dash_title"] = "System Dashboard",
                ["dash_health"] = "Health",
                ["dash_processes"] = "Processes",
                ["dash_uptime"] = "Uptime",
                ["dash_boost_ram"] = "Boost RAM",
                ["dash_boost_ram_desc"] = "Free up memory",
                ["dash_game_mode"] = "Game Mode",
                ["dash_game_mode_desc"] = "Optimize for gaming",
                ["dash_network_reset"] = "Network Reset",
                ["dash_network_reset_desc"] = "Flush DNS cache",
                ["dash_quick_actions"] = "Quick Actions",
                ["dash_top_processes"] = "Top Resource-Hungry Processes",
                ["dash_recommended"] = "Recommended Actions",
                ["dash_status_good"] = "System Running Smoothly",
                ["dash_status_moderate"] = "System Under Moderate Load",
                ["dash_status_heavy"] = "System Under Heavy Load!",
                ["dash_end"] = "End",
                ["dash_pid"] = "PID",
                ["dash_name"] = "Name",
                ["dash_memory"] = "Memory",
                ["dash_status"] = "Status",
                ["dash_action"] = "Action",

                // Performance
                ["perf_title"] = "Performance Tweaks",
                ["perf_subtitle"] = "Optimize your system for maximum performance",
                ["perf_system"] = "System",
                ["perf_memory"] = "Memory",
                ["perf_disk"] = "Disk",
                ["perf_gaming"] = "Gaming",
                ["perf_apply"] = "Apply Changes",
                ["perf_reset"] = "Reset All",
                ["perf_no_changes"] = "No changes to apply.",
                ["perf_applied"] = "tweak(s) applied successfully!",
                ["perf_reset_msg"] = "All tweaks in this category have been reset.",

                // Cleaner
                ["clean_title"] = "Junk Cleaner",
                ["clean_scan"] = "Scan Now",
                ["clean_scanning"] = "Scanning...",
                ["clean_scanning_system"] = "Scanning your system...",
                ["clean_complete"] = "Scan complete!",
                ["clean_no_junk"] = "Your system is clean! No junk files found.",
                ["clean_found"] = "Found {0} files ({1}) in {2} categories",
                ["clean_btn"] = "Clean Selected",
                ["clean_select_all"] = "Select All",
                ["clean_freed"] = "Cleanup complete! Freed {0}",
                ["clean_default"] = "Click 'Scan Now' to find junk files",
                ["clean_total"] = "Total",
                ["clean_files"] = "files",

                // Startup
                ["start_title"] = "Startup Manager",
                ["start_subtitle"] = "Control what runs when Windows starts",
                ["start_programs"] = "Startup Programs",
                ["start_services"] = "Windows Services",
                ["start_refresh"] = "Refresh",
                ["start_found"] = "Found {0} startup items and {1} services",

                // Network
                ["net_title"] = "Network Booster",
                ["net_subtitle"] = "Optimize your network connection and diagnose issues",
                ["net_ping_test"] = "Ping Test",
                ["net_one_click"] = "One-Click Fixes",
                ["net_flush_dns"] = "Flush DNS",
                ["net_reset_winsock"] = "Reset Winsock",
                ["net_renew_ip"] = "Renew IP",
                ["net_full_reset"] = "Full Reset",
                ["net_dns_switcher"] = "DNS Switcher",
                ["net_tcp_opt"] = "TCP/IP Optimization",
                ["net_apply_tcp"] = "Apply TCP Optimizations",
                ["net_adapters"] = "Network Adapters",
                ["net_found"] = "Found {0} network adapters",

                // Privacy
                ["priv_title"] = "Privacy Guard",
                ["priv_subtitle"] = "Control Windows telemetry and protect your privacy",
                ["priv_score"] = "Privacy Score",
                ["priv_max"] = "Max Privacy",
                ["priv_restore"] = "Restore Defaults",
                ["priv_apply"] = "Apply Changes",
                ["priv_applied"] = "{0} privacy settings applied successfully!",
                ["priv_max_msg"] = "All privacy protections enabled. Click Apply to save.",
                ["priv_restore_msg"] = "Privacy settings restored to Windows defaults. Click Apply to save.",

                // Tools
                ["tools_title"] = "Advanced Tools",
                ["tools_subtitle"] = "System utilities and diagnostic tools",
                ["tools_sysinfo"] = "System Info",
                ["tools_processes"] = "Processes",
                ["tools_benchmark"] = "Benchmark",
                ["tools_run_bench"] = "Run Benchmark",

                // Reports
                ["rep_title"] = "Reports & History",
                ["rep_export"] = "Export CSV",
                ["rep_clear"] = "Clear",
                ["rep_records"] = "{0} optimization records",

                // About
                ["about_title"] = "About Enumium",
                ["about_version"] = "Version 1.0.2",
                ["about_desc"] = "Professional Windows System Optimization Software",
                ["about_developer"] = "Developer",
                ["about_github"] = "GitHub",
                ["about_built_with"] = "Built with C# / WPF / .NET 8",
                ["about_rights"] = "All rights reserved.",
                ["about_features"] = "Features",
                ["about_features_list"] = "• Real-time system monitoring\n• 17+ performance tweaks\n• Intelligent junk cleaner\n• Startup manager\n• Network optimizer\n• Privacy guard\n• Benchmark tool\n• Optimization history",

                // Splash
                ["splash_title"] = "ENUMIUM",
                ["splash_subtitle"] = "System Optimizer",
                ["splash_loading"] = "Initializing...",
                ["splash_choose_lang"] = "Choose Language",

                // General
                ["apply"] = "Apply",
                ["cancel"] = "Cancel",
                ["ok"] = "OK",
                ["yes"] = "Yes",
                ["no"] = "No",
                ["close"] = "Close",
                ["running"] = "Running",
                ["not_responding"] = "Not Responding",
                ["safe"] = "Safe",
            },

            // ═══════════════════ RUSSIAN ═══════════════════
            ["ru"] = new Dictionary<string, string>
            {
                // Navigation
                ["nav_dashboard"] = "Панель",
                ["nav_performance"] = "Оптимизация",
                ["nav_cleaner"] = "Очистка",
                ["nav_startup"] = "Автозагрузка",
                ["nav_network"] = "Сеть",
                ["nav_privacy"] = "Приватность",
                ["nav_tools"] = "Инструменты",
                ["nav_reports"] = "Отчёты",
                ["nav_about"] = "О программе",

                // Dashboard
                ["dash_title"] = "Панель управления",
                ["dash_health"] = "Здоровье",
                ["dash_processes"] = "Процессы",
                ["dash_uptime"] = "Время работы",
                ["dash_boost_ram"] = "Ускорить ОЗУ",
                ["dash_boost_ram_desc"] = "Освободить память",
                ["dash_game_mode"] = "Игровой режим",
                ["dash_game_mode_desc"] = "Оптимизировать для игр",
                ["dash_network_reset"] = "Сброс сети",
                ["dash_network_reset_desc"] = "Очистить DNS кэш",
                ["dash_quick_actions"] = "Быстрые действия",
                ["dash_top_processes"] = "Самые ресурсоёмкие процессы",
                ["dash_recommended"] = "Рекомендуемые действия",
                ["dash_status_good"] = "Система работает стабильно",
                ["dash_status_moderate"] = "Система под средней нагрузкой",
                ["dash_status_heavy"] = "Система перегружена!",
                ["dash_end"] = "Завершить",
                ["dash_pid"] = "PID",
                ["dash_name"] = "Имя",
                ["dash_memory"] = "Память",
                ["dash_status"] = "Статус",
                ["dash_action"] = "Действие",

                // Performance
                ["perf_title"] = "Настройки производительности",
                ["perf_subtitle"] = "Оптимизируйте систему для максимальной производительности",
                ["perf_system"] = "Система",
                ["perf_memory"] = "Память",
                ["perf_disk"] = "Диск",
                ["perf_gaming"] = "Игры",
                ["perf_apply"] = "Применить",
                ["perf_reset"] = "Сбросить",
                ["perf_no_changes"] = "Нет изменений для применения.",
                ["perf_applied"] = "настроек применено успешно!",
                ["perf_reset_msg"] = "Все настройки в этой категории сброшены.",

                // Cleaner
                ["clean_title"] = "Очистка системы",
                ["clean_scan"] = "Сканировать",
                ["clean_scanning"] = "Сканирование...",
                ["clean_scanning_system"] = "Сканирование системы...",
                ["clean_complete"] = "Сканирование завершено!",
                ["clean_no_junk"] = "Система чиста! Мусор не найден.",
                ["clean_found"] = "Найдено {0} файлов ({1}) в {2} категориях",
                ["clean_btn"] = "Очистить выбранное",
                ["clean_select_all"] = "Выбрать все",
                ["clean_freed"] = "Очистка завершена! Освобождено {0}",
                ["clean_default"] = "Нажмите 'Сканировать' для поиска мусора",
                ["clean_total"] = "Итого",
                ["clean_files"] = "файлов",

                // Startup
                ["start_title"] = "Менеджер автозагрузки",
                ["start_subtitle"] = "Управляйте программами при запуске Windows",
                ["start_programs"] = "Автозагрузка",
                ["start_services"] = "Службы Windows",
                ["start_refresh"] = "Обновить",
                ["start_found"] = "Найдено {0} элементов автозагрузки и {1} служб",

                // Network
                ["net_title"] = "Оптимизация сети",
                ["net_subtitle"] = "Оптимизируйте сетевое соединение",
                ["net_ping_test"] = "Тест пинга",
                ["net_one_click"] = "Быстрые исправления",
                ["net_flush_dns"] = "Очистить DNS",
                ["net_reset_winsock"] = "Сброс Winsock",
                ["net_renew_ip"] = "Обновить IP",
                ["net_full_reset"] = "Полный сброс",
                ["net_dns_switcher"] = "Выбор DNS",
                ["net_tcp_opt"] = "Оптимизация TCP/IP",
                ["net_apply_tcp"] = "Применить TCP оптимизации",
                ["net_adapters"] = "Сетевые адаптеры",
                ["net_found"] = "Найдено {0} сетевых адаптеров",

                // Privacy
                ["priv_title"] = "Защита приватности",
                ["priv_subtitle"] = "Контролируйте телеметрию и защитите конфиденциальность",
                ["priv_score"] = "Уровень приватности",
                ["priv_max"] = "Макс. приватность",
                ["priv_restore"] = "По умолчанию",
                ["priv_apply"] = "Применить",
                ["priv_applied"] = "{0} настроек приватности применено!",
                ["priv_max_msg"] = "Все защиты включены. Нажмите Применить для сохранения.",
                ["priv_restore_msg"] = "Настройки восстановлены до стандартных. Нажмите Применить.",

                // Tools
                ["tools_title"] = "Инструменты",
                ["tools_subtitle"] = "Утилиты и диагностика системы",
                ["tools_sysinfo"] = "Информация",
                ["tools_processes"] = "Процессы",
                ["tools_benchmark"] = "Тест",
                ["tools_run_bench"] = "Запустить тест",

                // Reports
                ["rep_title"] = "Отчёты и история",
                ["rep_export"] = "Экспорт CSV",
                ["rep_clear"] = "Очистить",
                ["rep_records"] = "{0} записей оптимизации",

                // About
                ["about_title"] = "О программе Enumium",
                ["about_version"] = "Версия 1.0.2",
                ["about_desc"] = "Профессиональное ПО для оптимизации Windows",
                ["about_developer"] = "Разработчик",
                ["about_github"] = "GitHub",
                ["about_built_with"] = "Создано на C# / WPF / .NET 8",
                ["about_rights"] = "Все права защищены.",
                ["about_features"] = "Функции",
                ["about_features_list"] = "• Мониторинг системы в реальном времени\n• 17+ настроек производительности\n• Интеллектуальная очистка\n• Менеджер автозагрузки\n• Оптимизация сети\n• Защита приватности\n• Тест производительности\n• История оптимизаций",

                // Splash
                ["splash_title"] = "ENUMIUM",
                ["splash_subtitle"] = "Оптимизатор системы",
                ["splash_loading"] = "Инициализация...",
                ["splash_choose_lang"] = "Выберите язык",

                // General
                ["apply"] = "Применить",
                ["cancel"] = "Отмена",
                ["ok"] = "ОК",
                ["yes"] = "Да",
                ["no"] = "Нет",
                ["close"] = "Закрыть",
                ["running"] = "Работает",
                ["not_responding"] = "Не отвечает",
                ["safe"] = "Безопасно",
            }
        };

        public static string Get(string key)
        {
            if (Translations.TryGetValue(CurrentLanguage, out var dict) && dict.TryGetValue(key, out var val))
                return val;
            if (Translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enVal))
                return enVal;
            return key;
        }

        public static string Get(string key, params object[] args)
        {
            var template = Get(key);
            try { return string.Format(template, args); }
            catch { return template; }
        }
    }
}
