using ServiceStack.ServiceHost;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Route("/do/{target}/{action}/{x}/{y}/{z}")]
public class Do
{
    public string target { get; set; }
    public string action { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}