namespace RegionExtension.Database.Actions
{
    public static class ActionFactory
    {
        public const string AllowName = "Allow";
        public const string RemoveName = "Remove";
        public const string SetZName = "SetZ";
        public const string AllowGroupName = "AllowG";
        public const string RemoveGroupName = "RemoveG";
        public const string ChangeOwnerName = "ChangeOwner";
        public const string MoveName = "Move";
        public const string ResizeName = "Resize";
        public const string ProtectName = "Protect";

        public static IAction GetActionByName(string name, string args)
        {
            
            IAction action = null;
            switch(name)
            {
                case (AllowName):
                    action = new Allow(args);
                    break;
                case (RemoveName):
                    action = new Remove(args);
                    break;
                case (SetZName):
                    action = new SetZ(args);
                    break;
                case (AllowGroupName):
                    action = new AllowGroup(args);
                    break;
                case (RemoveGroupName):
                    action = new RemoveGroup(args);
                    break;
                case (ChangeOwnerName):
                    action = new ChangeOwner(args);
                    break;
                case (MoveName):
                    action = new Move(args);
                    break;
                case ResizeName:
                    action = new Resize(args);
                    break;
                case ProtectName:
                    action = new Protect(args);
                    break;
                case RenameName:
                    action = new Rename(args);
                    break;
            }
            return action;
        }
    }
}
