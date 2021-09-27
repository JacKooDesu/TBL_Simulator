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
        Action command;
        public Command(string id, Action command) : base(id)
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
        Action<T1> command;
        public Command(string id, Action<T1> command) : base(id)
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
        Action<T1, T2> command;
        public Command(string id, Action<T1, T2> command) : base(id)
        {
            this.command = command;
        }

        public void Invoke(T1 v1, T2 v2)
        {
            command.Invoke(v1, v2);
        }
    }

}