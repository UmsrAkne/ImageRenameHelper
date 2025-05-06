using System;

namespace ImageRenameHelper.ViewModels
{
    public interface IMessenger
    {
        event EventHandler<string> SystemMessagePublished;
    }
}