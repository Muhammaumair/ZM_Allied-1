using ZMAllied.Domain.Entities.Bilty;
namespace ZMAllied.Application.Interfaces 
{ 
    public interface IBiltyPrintService 
    { 
        byte[] Generate(Bilty bilty); 
    } 
}
