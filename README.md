# Region Extension
## EN
Plugin add more commands for better region management & add context parameters.
### Commands
#### RegionExt (/re, /region)
All commands integrated with /region cmd, also all default command replaced with plugin
- `rename <oldname> <newname>` - Set the newname of the region with given oldname.
- `move <regionname> <amount> <u/d/r/l> ` - Move region coordinate at the given direction. up, down, right, left
- `fastregion/fr <regionname> [ownername] [z] [protect]` - Defines the region with given points. Also allow set point with The Grand Design.
- `frbreak` - Break fastregion request.
- `clearm <regionname>` - Clear all allowed members at the given region.
- `setowner <regionname> <username>` - Set region owner.
#### RegionOwn (/ro, /regionown)
Commands for region owners.
- `list [page]` - Lists of all your regions.
- `allow <user> <region>` - Allows a user to a region.
- `remove <user> <region>` - Removes a user from a region.
- `giveown <user> <region>` - Change owner of region.
- `info <region>` - Displays several information about the given region.
#### RegionHistory (/rh, /regiohistory)
Commands for region history.
- `undo/u <count> [region]` - undo actions on region.
- `redo/r <count> [region]` - redo actions on region.
- `restore/res <regionname>` - restore region from deleted regions.
- `history/h [page] [region]` - gets history about region.
- `dellist/dl [page]` - get list of deleted regions.
### Contexts
Context uses for faster command input and result their result depends on external factors. Also works with /region
- `$this` - take regionname of player current region.
- `$myname` - take username of player account.
- `$near` - take nearest player name.

Example: /region info $this. return info about player current region.
### Config
- ContextSpecifier, default: "$"
- ContextAllow, default: true
- AutoCompleteSameName, default: true // if player define region with existing region name, plugin will automatically change new region name with specific format.

Example: if region name "rname" already exist, define name automatically changes into "rname:1".
- AutoCompleteSameNameFormat, default: "{0}:{1}" //{0} - region name, {1} - region number
### Permissions
- `tshock.admin.region` - allow use /regionext commands.
- `regionext.own` - allow use /regionown commands.
- `regionext.history` - allow use /regionhistory commands.

## RU
Плагин добавляет больше команд для лучшего использования регионов, также добавляет контекстные параметры.
### Команды
#### RegionExt (/re, /region)
Все команды интегрированны с /region, все станадртные команды также заменены со стороны плагина.
- `rename <oldname> <newname>` - Устанавливает новое имя (newname) для указанного региона (oldname).
- `move <regionname> <u/d/r/l> <amount>` - Перемещает координаты региона в указанном направлении. u - вверх, d - вниз, r - вправо, l - влево.
- `fastregion/fr <regionname> [ownername] [z] [protect]` - Задает регион с указанными параметрами и запрашивает точки региона. Зону можно указать с помощью The Grand Design.
- `frbreak` - Отменяет активный запрос на быстрый регион (fastregion).
- `clearm <regionname>` - Очищает список всех игроков, что могут строить в регионе (regionname).
- `setowner <regionname> <username>` - Задает нового владельца региона (username) для указаного региона.
- `contexts [page]` - Отображение всех доступных контекстных команд.
#### RegionOwn (/ro, /regionown)
Команды для владельцев регионов.
- `list` - Список всех регионов, которыми владеет игрок.
- `allow <user> <region>` - Добавление игрока в регион.
- `remove <user> <region>` - Удаление игрока из региона.
- `giveown <user> <region>` - Передача владения регионом другому игроку.
- `info <region>` - Отображение информации о регионе владельца.
- `contexts [page]` - Отображение всех доступных контекстных команд.
#### RegionHistory (/rh, /regiohistory)
Команды для истории регионов
- `undo/u <count> [region]` - отменяет действия на регионе.
- `redo/r <count> [region]` - возвращает действия на регион.
- `restore/res <regionname>` - восстанавливает регион из удаленны. Имя должно быть полностью индентичным.
- `history/h [page] [region]` - получает историю о регионе.
- `dellist/dl [page]` - лист удаленных регионов
### Контекст
Контекст используется для быстрого ввода команд без поиска необходимых параметров. Также работает с /region
- `$this` - берет имя региона в котором находится игрок.
- `$myname` - берет имя аккаунта игрока.
- `$near` - получает имя ближайшего игрока.

Пример: /region info $this. Вернет информацию о регионе, в котором находится игрок.
### Конфиг
- ContextSpecifier, default: "$" //Начальный символ для контекстных параметров.
- ContextAllow, default: true //Разрешает/запрещает использование контекста.
- AutoCompleteSameName, default: true // Если игрок задает имя для нового региона, которое уже используется, плагин автоматически заменяет имя в соответствии с форматом.

Пример: Если регион с именем "rname", уже существует, новое имя автоматически будет заменено на "rname:1".
- AutoCompleteSameNameFormat, default: "{0}:{1}" // Формат замены имени нового региона. {0} - имя региона, {1} - номер нового региона.
### Привилегии
- `tshock.admin.region` - позволяет использовать команды /regionext. 
- `regionext.own` - позволяет использовать команды /regionown.
- `regionext.history` - allow use /regionhistory commands.
