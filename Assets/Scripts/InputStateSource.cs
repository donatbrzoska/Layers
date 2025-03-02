public interface InputStateSource
{
    bool HasNext();
    InputState Next();
}