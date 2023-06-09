using System;
namespace TBL.Sys
{
    public interface ILoopable
    {
        void Enter();
        bool Update(float dt);
        void Exit();
    }
}