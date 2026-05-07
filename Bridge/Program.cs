public interface IRenderer
{
    void RenderShape(string shapeName);
}

public class VectorRenderer : IRenderer
{
    public void RenderShape(string shapeName)
    {
        Console.WriteLine($"Малює {shapeName} векторною графікою.");
    }
}

public class RasterRenderer : IRenderer
{
    public void RenderShape(string shapeName)
    {
        Console.WriteLine($"Малює {shapeName} растровою графікою.");
    }
}

public abstract class Shape
{
    protected IRenderer _renderer;

    protected Shape(IRenderer renderer)
    {
        _renderer = renderer;
    }

    public abstract void Draw();
}

public class Circle : Shape
{
    public Circle(IRenderer renderer) : base(renderer) { }
    public override void Draw() => _renderer.RenderShape("коло");
}

public class Square : Shape
{
    public Square(IRenderer renderer) : base(renderer) { }
    public override void Draw() => _renderer.RenderShape("квадрат");
}

public class Triangle : Shape
{
    public Triangle(IRenderer renderer) : base(renderer) { }
    public override void Draw() => _renderer.RenderShape("трикутник");
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        IRenderer vector = new VectorRenderer();
        IRenderer raster = new RasterRenderer();

        Shape circle = new Circle(vector);
        Shape square = new Square(raster);
        Shape triangle = new Triangle(vector);

        Console.WriteLine("Векторна графіка");

        circle.Draw();
        square.Draw();
        triangle.Draw();

        Console.WriteLine("\nРастрова графіка");

        Shape rasterTriangle = new Triangle(raster);
        rasterTriangle.Draw();
    }
}