namespace P4LU1API;

public class Afval
{
    public Guid GUID { get; set; }
    public string Label { get; set; }
    public float Confidence { get; set; }
    public double LocatieCoördinaten { get; set; }
    public string LocatieAdres {  get; set; }
    public DateTime Tijd {  get; set; }
    public int CameraID { get; set; }
    public float BoundingBoxLB { get; set; }
    public float BoundingBoxRB { get; set; }
    public float BoundingBoxCenter {  get; set; }
}
