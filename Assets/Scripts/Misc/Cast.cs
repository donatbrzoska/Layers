public class Cast
{
    public static int BoolToInt(bool b)
    {
        return b ? 1 : 0;
    }

    public static bool IntToBool(int i)
    {
        return i == 0 ? false : true;
    }
}