using System;

namespace TBL.Command
{
    public class CommandBase
    {
        string commandId;
        public string CommandId { get => commandId; }

        public CommandBase(string id)
        {
            commandId = id;
        }
    }

    public class Command : CommandBase
    {
        System.Action command;
        public Command(string id, System.Action command) : base(id)
        {
            this.command = command;
        }

        public void Invoke()
        {
            command.Invoke();
        }
    }

    public class Command<T1> : CommandBase
    {
        System.Action<T1> command;
        public Command(string id, System.Action<T1> command) : base(id)
        {
            this.command = command;
        }

        public void Invoke(T1 value)
        {
            command.Invoke(value);
        }
    }

    public class Command<T1, T2> : CommandBase
    {
        System.Action<T1, T2> command;
        public Command(string id, System.Action<T1, T2> command) : base(id)
        {
            this.command = command;
        }

        public void Invoke(T1 v1, T2 v2)
        {
            command.Invoke(v1, v2);
        }
    }

}