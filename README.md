# Region Extension
## EN
Plugin add more commands for better region management & add context parameters.
#### RegionExt (/re, /regionext)
All commands integrated with /region, also change all default commands.
- `move/mv <regionname> <amount> <u/d/r/l>` - Move region with given name in given direction.
- `setowner/so [useraccount] [region]` - Set region owner.
- `clearmembers/cm [regionname]` - Remove all members from region.
- `fastregion/fr <regionname> [ownername] [z] [protect]` - Create new region with two given point and params.
- `frbreak` - Breaks fast region request.
- `ownerlist/ol [page] [username]` - Get list of regions in which the given player is owner.
- `listact/la [page]` - Get list of last active regions.
- `listrequest/lr [page]` - Lists all region requests.
- `requestinfo/ri [region] [page]` - Displays several information about the given request.
- `requestaccept/ra [region]` - Accept region request.
- `requestdeny/rd [region]` - Deny region request.\
P.S. Requests are sent from the player who created the region via the /regionown command, the player who has access to the main commands must check the request, otherwise the region will be deleted according to the settings.
#### RegionOwn (/ro, /regionown)
Commands for owners of regions. The commands are identical to the main ones, however, each checks whether the player is the owner of the region
- `setowner/so [useraccount] [region]` - Set region owner.
- `clearmembers/cm [regionname]` - Remove all members from region.
- `ownerlist/ol [page]` - Get list of regions in which you is owner
- `allow/a <useraccount> [region]` - Allows a user to a region.
- `remove/r <useraccount> [region]` - Removes a user from a region.
- `info/i [region] [page]` - Displays several information about the given region.
- `set <1/2>` - Sets the temporary region points.
- `define/d <name>` - Defines the region with the given name and send request.
- `delete/del [region]` - Deletes the given region.
- `fastregion/fr <region>` - Create new region with two given point and params. Also send request.
- `fastregionbreak/frb` - Breaks fast region request.
#### RegionHistory (/rh, /regiohistory)
Commands for region history.
- `undo/u <count> [region]` - Undo actions on region.
- `redo/r <count> [region]` - Redo actions on region.
- `restore/res <regionname>` - Restore region from deleted regions.
- `restoreuser/resu <user> [count]` - Restore region from deleted regions with user.
- `history/h [page] [region]` - Gets history about region.
- `dellist/dl [page]` - Get list of deleted regions.
P.S. The maximum buffer for deleted regions is 64 entries.
#### RegionTrigger (/rt, /regiontrigger)
Commands for region triggers.
- `add/a <region> <event> <trigger>` - Adds trigger to the region.
- `delete/d <region> <id>` - Deletes trigger from the region.
- `info/i [region] [page]` - Info about triggers of the region.
- `list/l [page]` - List all available triggers.
- `helptrigger/ht <trigger> [page]` - Returns all info about given trigger.
- `eventlist/el [page]` - List all available events.
- `conditionlist/cl [page]` - List available conditions.
- `addcond/ac <region> <condition> [ids...]` - Adds conditions to the trigger.
- `removecond/rc <region> <condition> [ids...]` - Removes condition from the trigger.
- `clear/c [region]` - Clears triggers from the region.\
Trigger - an action that takes place on a given event.\
Example: By setting the trigger /rt a $t e msg Hello world!\
To the player who entered (event - e/enter) in region ($t) will be sent a message (action - msg/message) "Hello world!".\
Conditions - the condition under which the trigger will work.\
Example: By setting the condition /rt ac $t !a 0\
A message trigger set in a region and given an id of 0 (can be checked in "/rt i" command) will only be sent to a player who is not allowed to the region (the condition - a/allowed, '!' in front of the name - makes the condition opposite).\
If a trigger has more than 1 condition, all conditions must be true for the trigger to execute.
#### RegionProperty (/rp, /regionproperty)
- `add/a <region> <property>` - Adds property to the region.
- `remove/r <region> <property>` - Remove property from the region.
- `list/l [page]` - List available properties.
- `info/i [page] [region]` - Info about property of the region
- `helpproperty/hp <property> [page]` - Returns all info about given property.
- `addcond/ac <region> <condition> <property>` - Adds condition to the property.
- `removecond/rc <region> <condition> <property>` - Removes condition from the property.
- `clear/c [region]` - Clears properties from the region.\
Properties - certain rules by which the region exists and events change.\
Example: By setting the property - /rp a $t ap\
A player located in the region ($t) automatically turns on PvP mode and is not able to turn it off (property - ap/alwayspvp)\
Conditions also work on properties, however not all properties are subject to them, and not all conditions will be valid for a property\
Example: By setting the condition - /rp ac $t !a ap\
The property will only affect players not added to the region.
#### Available triggers.
- `command/cmd <command>` - Command trigger.
> There are constants for commands:
> @r - replaced by the name of the region
> @p - replaced by the player who activated trigger
- `push/p` - Pushes player from region.
- `packet/pa <int> [text] [data...]` - Send packet to the player.
- `message/msg <text...>` - Send message to the player.
- `spawnnpc/spawnmob/sn/sm <npc> [count] [x] [y] [health] [strength]` - Spawns npc.
- `spawnproj/sp <projectile> [count] [damage] [knockback] [x] [y] [speedX] [speedY]` - Spawns projectile.
- `giveitem/spawnitem/g/si <item> [stack] [prefix] [x] [y] [damage] [usetime] [projectile]` - Gives items to the player.
- `tppos <x> <y>` - Teleports player to the position.
- `warp <warp>` - Teleports player to the warp.
- `kill/k` - Kills player.
- `buff/b <buff> [time]` - Buffs player. Time in Seconds.\
In coordinates, you can set a function that is calculated when the trigger fires.\
Example: The trigger is /rt a $t e g 1 1 0 px+1 py+1. Throws out an iron pick at the player's coordinates.\
All available features:
- `px, py` - will return the coordinate of the player who executed the trigger.
- `cx, cy` - returns the coordinate of the left-upper corner of the region.
- `w, h` - will return the width or height of the region.
- `ri` - will return a random number from 0 to int.maxValue.
- `rd` - will return a random number from 0 to 1.
- `lx, ly` - will be replaced by the local coordinate of the player in the region that set the trigger.
- `gx, gy` - will be replaced by the coordinate of the player who set the trigger.
#### Available events
- `onenter/enter/e` - Activates when player enters in region.
- `onleave/leave/l` - Activates when player leaves from region.
- `onin/in/i` - Activates while player in the region.
- `onpvpon/pvpon` - Activates when players Pvp enabled.
- `onpvpoff/pvpoff` - Activates when players Pvp disabled.\
Update happens every half second
#### Available conditions
- `allowed/a`  - If player is allowed in the region.
- `exact/e <count>` - If players count in the region.
- `less/l <count>` - If less players in the region than count.
- `more/m <count>` - If more players in the region than count.
- `owner/o`  - If player is owner of the region.
- `pause/p [time]` - Pauses trigger in given time after activation. Format: 0d0h0m0s
- `playerpause/pp [time]` - Pauses trigger for player in given time after activation. Format: 0d0h0m0s
- `delay/d [time] [flag]` - Activates the trigger only after the given time.
- `playerdelay/pd [time] [flag]` - Activates the trigger only after the given time for the player.
- `recheck/rc` - Rechecks the actual presence of the player in region.
> Flags - additional conditions under which the delay trigger is activated.
> - `-f` - The trigger is activated at the end of the delay, regardless of the presence of a player in the region. Default.
> - `-i` - The trigger is activated if the player is in the region at the end of the delay.
> - `-a` - The trigger is activated if the player was in the region during the delay.
#### Available properties
- `alwayspvp/ap` - Activates player pvp and prevents trying to change it.
- `banhostile/bh` - Removes any hostile NPCs and projectiles from region, and prevents bosses from entering the region.
- `clearitems/ci` - Clears items from region.
- `maxspawn/ms <ratio>` - Rewrite near NPCs count for players.
> Property changes the number of NPCs calculated by the game by multiplying by a factor, thus allowing you to increase or decrease the actual spawning of NPCs near the player in the region. Example: If there are 10 NPCs near the player and the ratio is 0.5, then 10 NPCs will be counted as 5, increasing spawn, with 1.5 - 10 NPCs will be counted as 15, decreasing spawn. If set to 0, then NPCs around the player will not spawn.
- `nopvp/np` - Deactivates player pvp and prevents trying to change it.
- `spawnrewrite/sr <npcs...>` - Rewrites npc spawn in the region.
> Only NPCs in the region are changed, regardless of the presence of the player or his position, the list is given in the format {Name or id}:{Weight} - where weight is the probability of a particular NPC appearing, relative to the weight of other entered NPCs. Example: With a list of 1:1 3:0.5 - 1 zombie will appear for every 2 slimes, or by probabilities 1/1.5 = 66% 0.5/1.5 = 33%. Default weight - 1
- `projban/pb <projs...>` - Prevents projectile creation from player.
- `itemban/ib <items...>` - Ban items in the region.
#### Utils commands
- `context` - Return all available context commands.
- `reperm` - Returns all permissions used by Region Extension plugin.
- `reloc` - Changes Region extension localization. Available EN/RU.
- `triggerignore/ti` - Ignores any trigger and some property activation.
## RU
Плагин добавляет больше команд для лучшего использования регионов, также добавляет контекстные параметры.
### Команды
#### RegionExt (/re, /region)
Все команды интегрированны с /region, все стандартные команды также заменены со стороны плагина.
- `move/mv <regionname> <amount> <u/d/r/l>` - Перемещает координаты региона в указанном направлении. u - вверх, d - вниз, r - вправо, l - влево.
- `setowner/so [useraccount] [region]` - Задает нового владельца региона (username) для указаного региона.
- `clearmembers/cm [regionname]` - Очищает список всех игроков, что могут строить в регионе.
- `fastregion/fr <regionname> [ownername] [z] [protect]` - Задает регион с указанными параметрами и запрашивает точки региона. Зону можно указать с помощью The Grand Design.
- `frbreak` - Отменяет активный запрос на быстрый регион (fastregion).
- `ownerlist/ol [page] [username]` - Перечисляет все регионы с заданным владельцем
- `contexts [page]` - Отображение всех доступных контекстных команд.
- `listact/la [page]` - Перечисляет все регионы в порядке активности.
- `listrequest/lr [page]` - Отображает все активные запросы.
- `requestinfo/ri [region] [page]` - Отображает информацию о запросе.
- `requestaccept/ra [region]` - Принимает запрос.
- `requestdeny/rd [region]` - Отклоняет запрос.\
P.S. Запросы отправляются от игрока, который создает регион через команды /regionown, игрок, имеющий доступ к основным командам, должен подтвердить запрос, иначе запрошенный регион будет удален согласно настройкам. 
#### RegionOwn (/ro, /regionown)
Команды для владельцев регионов. Команды индентичны основным, однако в каждой проверяется является ли игрок владельцем региона
- `setowner/so [useraccount] [region]` - Устанавливает владельца региона.
- `clearmembers/cm [regionname]` - Удаляет всех игроков из региона.
- `ownerlist/ol [page]` - Отображает все регионы, в которых вы владелец.
- `allow/a <useraccount> [region]` - Добавляет игрока в регион.
- `remove/r <useraccount> [region]` - Удаляет пользователя из региона.
- `info/i [region] [page]` - Отображает информацию о регионе.
- `set <1/2>` - Устанавливает точку для создания региона.
- `define/d <name>` - Создает регион с установленными точками и заданным именем, и отправляет запрос.
- `delete/del [region]` - Удаляет заданный регион.
- `fastregion/fr <region>` - Создает регион и запрашивает установление точек. Также отправляет запрос.
- `fastregionbreak/frb` - Удаляет запрос на быстрый регион.
#### RegionHistory (/rh, /regiohistory)
Команды для истории регионов.
- `undo/u <count> [region]` - Отменяет действие над регионом.
- `redo/r <count> [region]` - Восстанавливает действие над регионом.
- `restore/res <regionname>` - Восстанавливает регион из удаленных.
- `restoreuser/resu <user> [count]` - Восстанавливает регионы из удаленных по пользователю.
- `history/h [page] [region]` - Получает всю историю действий на регион.
- `dellist/dl [page]` - Перечисляет все удаленные регионы.\
P.S. Максимальный буффер для удаленных регионов - 64 записи.
#### RegionTrigger (/rt, /regiontrigger)
Команды для триггеров регионов.
- `add/a <region> <event> <trigger>` - Добавляет триггер к региону.
- `delete/d <region> <id>` - Удаляет триггер из региона.
- `info/i [region] [page]` - Информация о триггерах в регионе.
- `list/l [page]` - Отображает все доступные триггеры.
- `helptrigger/ht <trigger> [page]` - Возвращает всю информацию о триггере.
- `eventlist/el [page]` - Перечисляет все доступные события.
- `conditionlist/cl [page]` - Перечисляет все доступные условия.
- `addcond/ac <region> <condition> [ids...]` - Добавляет условие к триггеру региона.
- `removecond/rc <region> <condition> [ids...]` - удаляет условие с триггера.
- `clear/c [region]` - Удаляет все триггеры из региона.\
Триггер - действие, совершающееся при заданном событии.\
Пример: Задав триггер /rt a $t e msg Привет мир!\
Игроку, который зашел (событие - e/enter) в регион ($t), отправится сообщение (действие - msg/message) "Привет мир!".\
Условия - условие при котором триггер сработает.\
Пример: Задав условие /rt ac $t !a 0\
Триггер сообщения, заданный в регионе и получивший id - 0 (узнается в команде /rt i), отправится только игроку, который не добавлен в регион (условие - a/allowed, ! перед названием - делает выполнение условие противоположным).\
Если у триггера больше 1 условия, необходимо чтоб все условия были верными для выполнения триггера.
#### RegionProperty (/rp, /regionproperty)
- `add/a <region> <property>` - Добавляет свойство региону.
- `remove/r <region> <property>` - Удаляет свойство из региона.
- `list/l [page]` - Перечисляет все доступные свойства.
- `info/i [page] [region]` - Отображает информацию о свойствах региона.
- `helpproperty/hp <property> [page]` - Отображает информацию о свойстве.
- `addcond/ac <region> <condition> <property>` - Добавляет условие к свойству региона.
- `removecond/rc <region> <condition> <property>` - Удаляет условие из свойства
- `clear/c [region]` - Удаляет все свойства из региона.\
Свойства - определенные правила по которым существует регион и изменяются события.\
Пример: Задав свойство - /rp a $t ap\
У игрока, находящегося в регионе ($t), автоматически включается режим PvP и он не способен его выключить (свойство - ap/alwayspvp)\
Условия также действуют на свойства, однако не все свойства подверженны им, и не все условия будут корректны для свойства\
Пример: Задав условие - /rp ac $t !a ap\
Свойство будет действовать только на игроков, не добавленных в регион.
#### Доступные триггеры
- `command/cmd <command>` - Вызывает команду.
> Присутствуют константы для команд:
> @r - заменится на название региона
> @p - заменится на игрока, активировавшего триггер
- `push/p` - Выталкивает игрока из региона.
- `packet/pa <int> [text] [data...]` - Отправляет пакет игроку.
- `message/msg <text...>` - Отправляет сообщение игроку.
- `spawnnpc/spawnmob/sn/sm <npc> [count] [x] [y] [health] [strength]` - Призывает НИПа.
- `spawnproj/sp <projectile> [count] [damage] [knockback] [x] [y] [speedX] [speedY]` - Спавнит снаряд.
- `giveitem/spawnitem/g/si <item> [stack] [prefix] [x] [y] [damage] [usetime] [projectile]` - Дает предмет игроку.
- `tppos <x> <y>` - Телепортирует игрока на позицию.
- `warp <warp>` - Телепортирует игрока на варп.
- `kill/k` - Убивает игрока.
- `buff/b <buff> [time]` - Баффает игрока. Время в секундах.\
В координатах можно задать функцию, которая просчитывается когда срабатывает триггер.\
Пример: Триггер - /rt a $t e g 1 1 0 px+1 py+1. Выкинет железную кирку по координатам игрока.\
Все доступные функции:
- `px, py` - вернет координату игрока, выполнившего триггер.
- `cx, cy` - вернет координату левого-верхнего угла региона.
- `w, h` - вернет ширину или высоту региона.
- `ri` - вернет случайное число от 0 до int.maxValue.
- `rd` - вернет случайное число от 0 до 1.
- `lx, ly` - заменится на локальную координату игрока в регионе, задавшего триггер.
- `gx, gy` - заменится на координату игрока, задавшего триггер.
#### Доступные события
- `onenter/enter/e` - Активируется когда игрок заходит в регион.
- `onleave/leave/l` - Активируется когда игрок выходит из региона.
- `onin/in/i` - Активируется пока игрок находится в регионе.
- `onpvpon/pvpon` - Активируется когда включается режим Пвп игрока.
- `onpvpoff/pvpoff` - Активируется когда отключается режим Пвп игрока.\
Обновление происходит каждые пол секунды
#### Доступные условия 
- `allowed/a`  - Если игрок добавлен в регион.
- `exact/e <count>` - Если игроков в регионе - заданное количество.
- `less/l <count>` - Если игроков в регионе меньше заданного количества.
- `more/m <count>` - Если игроков в регионе больше заданного количества.
- `owner/o`  - Если игрок - владелец региона.
- `pause/p [time]` - Останавливает триггер на заданное время. Формат: 0d0h0m0s
- `playerpause/pp [time]` - Останавливает триггер на заданное время для игрока. Формат: 0d0h0m0s
- `delay/d [time] [flag]` - Активирует триггер только после заданного времени.
- `playerdelay/pd [time] [flag]` - Активирует триггер только после заданного времени для игрока.
- `recheck/rc` - Перепроверяет действительное наличие игрока в регионе.
> Флаги - дополнительные условия при котором триггер задержки активируется.
> - `-f` - Триггер активируется по окончанию задержки независимо от наличия игрока в регионе. Стандартный.
> - `-i` - Триггер активируется если игрок в момент окончания задержки находится в регионе.
> - `-a` - Триггер активируется если игрок всё время задержки находился в регионе.
####  Доступные свойства
- `alwayspvp/ap` - Включает режим PvP игрока и предотвращает его изменение.
- `banhostile/bh` - Удаляет всех враждебных НИПов и снаряды, также не позволяет боссу зайти в регион.
- `clearitems/ci` - Чистит предметы из региона.
- `maxspawn/ms <ratio>` - Переписывает количество НИПов возле игрока.
> Де-факто свойство изменяет количество НИПов просчитанное игрой, умножая на коэффициент, тем самым позволяя увеличить или уменьшить фактическое появление НИПов вокруг игрока в регионе. Пример: Если вокруг игрока 10 НИПов и коэффициент - 0.5, то 10 нипов будут считаться как 5, увеличивая появление, при 1.5 - аналогично 10 НИПов будут считаться как 15, уменьшая появление. Если задано 0, то НИПы вокруг игрока появляться не будут.
- `nopvp/np` - Выключает режим PvP игрока и предотвращает его изменение.
- `spawnrewrite/sr <npcs...>` - Изменяет естественное появление НИПов в регионе.
> Изменяются только НИПы в регионе, независимо от наличия игрока или его положения, список задается в формате {Имя или id}:{Вес} - где вес, это вероятность появления конкретного НИПа, относительно веса других вписанных НИПов. Пример: При списке 1:1 3:0.5 - на каждых 2-х слаймов будет появляться 1 зомби или по вероятностям 1/1.5 = 66% 0.5/1.5 = 33%. Стандартный вес - 1
- `projban/pb <projs...>` - Предотвращает использование снаряда игроком.
- `itemban/ib <items...>` - Предотвращает использование предмета игроком.
#### Вспомогательные команды
- `context` - отображает все доступные контекстные команды.
- `reperm` - отображает все привилегии плагина.
- `reloc` - изменяет локализацию плагина. Доступны EN/RU.
- `triggerignore/ti` - игнорирует триггеры и некоторые свойства.
### Контекст
Контекст используется для быстрого ввода команд без поиска необходимых параметров.
- `$this/$t` - берет имя региона в котором находится игрок.
- `$myname/$mn` - берет имя аккаунта игрока.
- `$near/$n` - получает имя ближайшего игрока.

Пример: /region info $this. Вернет информацию о регионе, в котором находится игрок.
### Permission/Привилегии
- `tshock.admin.region` - /regionext /re /region
- `regionext.own` - /regionown /ro
- `regionext.history` - /regionhistory /ri
- `regionext.trigger` - /regiontrigger /rt
- `regionext.property` - /regionproperty /rp
- `regionext.trigger.ignore` - /triggerignore /ti
- `regionext.trigger.sendpacket` - sendpacket/sp
- `regionext.trigger.message` - message/msg
- `regionext.trigger.push` - push/p
- `regionext.trigger.command` - command/cmd
- `regionext.trigger.warp` - warp
- `regionext.trigger.spawnnpc` - spawnnpc/spawnmob/sn/sm
- `regionext.trigger.giveitem` - giveitem/spawnitem/g/si
- `regionext.trigger.tppos` - tppos
- `regionext.trigger.spawnproj` - spawnproj/sp
- `regionext.trigger.kill` - kill/k
- `regionext.trigger.buff` - buff/b
- `regionext.property.pvp` - alwayspvp/ap nopvp/np
- `regionext.property.banhostile` - banhostile/bh
- `regionext.property.spawnrewrite` - spawnrewrite/sr
- `regionext.property.projban` - projban/pb
- `regionext.property.itemban` - itemban/ib
- `regionext.property.maxspawn` - maxspawn/m
## Config
```
{
  "ContextSpecifier": "$", //Start symbol for context/Начальный символ для контекстных параметров.
  "ContextAllow": true, //Allows/disallow use context/Разрешает/запрещает использование контекста.
  "AutoCompleteSameName": true, //Use autonaming on region define/Использовать авто имя когда игрок объявляет регион.
  "AutoCompleteSameNameFormat": "{0}:{1}", //Name format {0}-name {1}-num/Формат имени - {0}-имя {1}-число
  "NotificationPeriod": "10m", //Notification period for requests format: 0d0h0m0s/Период оповещения о запросах
  "DefaultLocalization": "EN", //Default localization for commands/Стандартная локализация для команд
  "BannedTriggerCommands": [ //Banned commands for trigger/Запрещенные команды для триггеров
    "group",
    "user"
  ],
  "RequestSettings": [
    {
      "GroupName": "default", //Used group/Используемая группа
      "MaxRequestCount": 3, //Max requests by player/ Максимальное количество запрошенных регионов
      "RequestTime": "3d", //Time on which region will be deleted or approved/Время за которое регион будет удален или подтвержден
      "AutoApproveRequest": false, //Approve region on time end or delete/Подтверждать регион по окончанию временного промежутка или удалять.
      "MaxRequestArea": 10000, //Max area (width*height) of requested region/Максимальная площадь (ширина*высота) запрошенного региона
      "MaxRequestHeight": 100, //Max height of requested region/Максимальная высота запрошенного региона.
      "MaxRequestWidth": 100, //Max Width of requested region/Максимальная ширина запрошенного региона.
      "ProtectRequestedRegion": true, //Protect requested region/Защита запрошенных регионов.
      "DefaultRequestZ": 0 //Default requested region order/Стандартный приоритет запрошенного региона
    },
    {
      "GroupName": "superadmin",
      "MaxRequestCount": 0, //0 - infinite/бесконечность
      "RequestTime": "0s", //0 - infinite/бесконечность
      "AutoApproveRequest": true,
      "MaxRequestArea": 0, //0 - infinite/бесконечность
      "MaxRequestHeight": 0, //0 - infinite/бесконечность
      "MaxRequestWidth": 0, //0 - infinite/бесконечность
      "ProtectRequestedRegion": true,
      "DefaultRequestZ": 0 
    }
  ]
}
```
