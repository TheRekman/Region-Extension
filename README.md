# Region Extension
## EN
Plugin add more commands for better region management & add context parameters.
### Commands
- //re rename \<oldname> \<newname> - Set the newname of the region with given oldname.
- //re move \<regionname> \<u/d/r/l> \<amount> - Move region coordinate at the given direction. up, down, right, left
- //re fastregion/fr \<regionname> [ownername] [z] [protect] - Defines the region with given points. Also allow set point with The Grand Design.
- //re frbreak - Break fastregion request.
- //re clearm \<regionname> - Clear all allowed members at the given region.
- //re setowner \<regionname> \<username> - Set region owner.
### Contexts
Context uses for faster command input and result their result depends on external factors. Also works with /region
- $this - take regionname of player current region.
- $myname - take username of player account.

Example: /region info $this. return info about player current region.
### Config
- ContextSpecifier, default: "$"
- ContextAllow, default: true
- AutoCompleteSameName, default: true // if player define region with existing region name, plugin will automatically change new region name with specific format.

Example: if region name "rname" already exist, define name automatically changes into "rname:1".
- AutoCompleteSameNameFormat, default: "{0}:{1}" //{0} - region name, {1} - region number
### Permissions
Plugin dont use new permissions. All players with manageregion permission, already can use commands and contexts.
## RU
Плагин добавляет больше команд для лучшего использования регионов, также добавляет контекстные параметры.
### Комманды
- //re rename \<oldname> \<newname> - Устанавливает новое имя (newname) для указанного региона (oldname).
- //re move \<regionname> \<u/d/r/l> \<amount> - Перемещает координаты региона в указанном направлении. u - вверх, d - вниз, r - вправо, l - влево.
- //re fastregion/fr \<regionname> [ownername] [z] [protect] - Задает регион с указанными параметрами и запрашивает точки региона. Зону можно указать с помощью The Grand Design.
- //re frbreak - Отменяет активный запрос на быстрый регион (fastregion).
- //re clearm \<regionname> - Очищает список всего игроков, что могут строить в регионе (regionname).
- //re setowner \<regionname> \<username> - Задает нового владельца региона (username) для указаного региона.
### Контекст
Контекст используется для быстрого ввода команд без поиска необходимых параметров. Также работает с /region
- $this - берет имя региона в котором находится игрок.
- $myname - берет имя аккаунта игрока.

Пример: /region info $this. Вернет информацию о регионе, в котором находится игрок.
### Конфиг
- ContextSpecifier, default: "$" //Начальный символ для контекстных параметров.
- ContextAllow, default: true //Разрешает/запрещает использование контекста.
- AutoCompleteSameName, default: true // Если игрок задает имя для нового региона, которое уже используется, плагин автоматически заменяет имя в соответствии с форматом.

Пример: Если регион с именем "rname", уже существует, новое имя автоматически будет заменено на "rname:1".
- AutoCompleteSameNameFormat, default: "{0}:{1}" // Формат замены имени нового региона. {0} - имя региона, {1} - номер нового региона.
### Привилегии
Плагин не использует новые привилегии. Все игроки, уже имеющие доступ к изменению регионов, могут использовать возможности плагина.

