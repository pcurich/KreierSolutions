namespace Ks.Batch.Util
{
    public interface IBatchContainer
    {
        bool Start();

        bool Stop();

        bool Pause();

        bool Continue();

        void CustomCommand(int commandNumber);
    }
}