# Region Extension
## EN
Plugin add more commands for better region management & add context parameters.
IDK
New commands from 1.2
//re fully transfer in /region or /re
/ro now start wthout slash
/rh - history
user help for see all commands
### Commands
#### RegionExt (//re, //regionext)
- `//re rename <oldname> <newname>` - Set the newname of the region with given oldname.
- `//re move <regionname> <u/d/r/l> <amount>` - Move region coordinate at the given direction. up, down, right, left
- `//re fastregion/fr <regionname> [ownername] [z] [protect]` - Defines the region with given points. Also allow set point with The Grand Design.
- `//re frbreak` - Break fastregion request.
- `//re clearm <regionname>` - Clear all allowed members at the given region.
- `//re setowner <regionname> <username>` - Set region owner.
- `//re contexts [page]` - Show available contexts command.
#### RegionOwn (//ro, //regionown)
Commands for region owners.
- `//ro list [page]` - Lists of all your regions.
- `//ro allow <user> <region>` - Allows a user to a region.
- `//ro remove <user> <region>` - Removes a user from a region.
- `//ro giveown <user> <region>` - Change owner of region.
- `//ro info <region>` - Displays several information about the given region.
- `//ro contexts [page]` - Show available contexts command.
### Contexts
Context uses for faster command input and result their result depends on external factors. Also works with /region
- `$this` - take regionname of player current region.
- `$myname` - take username of player account.

Example: /region info $this. return info about player current region.
### Config
- ContextSpecifier, default: "$"
- ContextAllow, default: true
- AutoCompleteSameName, default: true // if player define region with existing region name, plugin will automatically change new region name with specific format.

Example: if region name "rname" already exist, define name automatically changes into "rname:1".
- AutoCompleteSameNameFormat, default: "{0}:{1}" //{0} - region name, {1} - region number
### Permissions
- `tshock.admin.region` - allow use //regionext commands.
- `regionext.own` - allow use //regionown commands.

## RU
Плагин добавляет больше команд для лучшего использования регионов, также добавляет контекстные параметры.
### Команды
#### RegionExt (//re, //regionext)
- `//re rename <oldname> <newname>` - Устанавливает новое имя (newname) для указанного региона (oldname).
- `//re move <regionname> <u/d/r/l> <amount>` - Перемещает координаты региона в указанном направлении. u - вверх, d - вниз, r - вправо, l - влево.
- `//re fastregion/fr <regionname> [ownername] [z] [protect]` - Задает регион с указанными параметрами и запрашивает точки региона. Зону можно указать с помощью The Grand Design.
- `//re frbreak` - Отменяет активный запрос на быстрый регион (fastregion).
- `//re clearm <regionname>` - Очищает список всех игроков, что могут строить в регионе (regionname).
- `//re setowner <regionname> <username>` - Задает нового владельца региона (username) для указаного региона.
- `//re contexts [page]` - Отображение всех доступных контекстных команд.
#### RegionOwn (//ro, //regionown)
Команды для владельцев регионов.
- `//ro list` - Список всех регионов, которыми владеет игрок.
- `//ro allow <user> <region>` - Добавление игрока в регион.
- `//ro remove <user> <region>` - Удаление игрока из региона.
- `//ro giveown <user> <region>` - Передача владения регионом другому игроку.
- `//ro info <region>` - Отображение информации о регионе владельца.
- `//ro contexts [page]` - Отображение всех доступных контекстных команд.
### Контекст
Контекст используется для быстрого ввода команд без поиска необходимых параметров. Также работает с /region
- `$this` - берет имя региона в котором находится игрок.
- `$myname` - берет имя аккаунта игрока.

Пример: /region info $this. Вернет информацию о регионе, в котором находится игрок.
### Конфиг
- ContextSpecifier, default: "$" //Начальный символ для контекстных параметров.
- ContextAllow, default: true //Разрешает/запрещает использование контекста.
- AutoCompleteSameName, default: true // Если игрок задает имя для нового региона, которое уже используется, плагин автоматически заменяет имя в соответствии с форматом.

Пример: Если регион с именем "rname", уже существует, новое имя автоматически будет заменено на "rname:1".
- AutoCompleteSameNameFormat, default: "{0}:{1}" // Формат замены имени нового региона. {0} - имя региона, {1} - номер нового региона.
### Привилегии
- `tshock.admin.region` - позволяет использовать команды //regionext. 
- `regionext.own` - позволяет использовать команды //regionown.

