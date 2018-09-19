# CityStats
Задание: разработать консольное приложение, которое при запуске из командной строки принимает следующие параметры: 
CityStats.exe <input_mode> <input_address>
Где <input_mode> и <input_address> могут принимать следующие значения:
1. filesystem, в этом режиме <input_address> указывает путь к директории. Приложение обрабатывает все файлы, содержащиеся в этой директории 
(возможны вложенные директории);
2. http, в этом режиме <input_address> указывает путь к файлу, в котором перечислены http адреса входных документов (один адрес в каждой 
строке). Приложение скачивает и обрабатывает все документы, указанные в этом файле.

Формат входных файлов один и тот же для обоих режимов запуска:
1. Файлы в формате UTF-8;
2. Каждая строка содержит строку и число, разделенные символом ‘,’. Первое значение содержит название города, второе – количество 
жителей в городе;
3. Имя города может быть на любом языке;
4. Имя города может быть использовано более одного раза;
5. Города «минск» и «Минск» считаются одним и тем же городом.

На выходе приложение формирует файл output.txt (перезаписывает, если такой уже существует). Файл должен иметь такой же формат, как и 
входные файлы. В результирующем файле не должно быть повторения имен городов. При этом числовой результат для города должен быть равен 
сумме всех чисел по всем файлам для этого города.

Требования к приложению и коду:
1. Обработка файлов в обоих режимах производится параллельно, максимум N файлов одновременно (N конфигурируется в App.config);
2. В случае ошибки входных данных, программа логирует сообщение об ошибке в консоль и заканчивает работу. Возможные ошибки: 
несуществующая директория для режима filesystem, access denied для чтения директории/файла в режиме filesystem, несуществующее имя 
входного файла для режима http, ошибка загрузки файла по http, неверный формат строки файла.

Код должен быть покрыт юнит-тестами в необходимом для Production-версии объёме.

!!!!! В качестве структуры для хранения данных о городах лучше использовать dictionary. В данной реализации была написана своя версия 
словаря (класс Cities).
