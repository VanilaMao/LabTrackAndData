
namespace Lab.UICommon.Utilities
{
    public interface IDialogService
    {
        string OpenDialogFile(string filter);
        string SaveDialogFile(string filter);
    }
}
