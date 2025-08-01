namespace ScottPlot.Plottables;

public class Radar() : IPlottable, IManagesAxisLimits
{
    /// <summary>
    /// The polar axis drawn beneath each radar series polygon
    /// </summary>
    public PolarAxis PolarAxis { get; set; } = new() { Rotation = Angle.FromDegrees(90) };

    /// <summary>
    /// A collection of RadarSeries, each of which hold a set of values and the styling information that controls how the shape is rendered
    /// </summary>
    public List<RadarSeries> Series { get; } = [];

    /// <summary>
    /// Enable this to modify the axis limits at render time to achieve "square axes"
    /// where the units/px values are equal for horizontal and vertical axes, allowing
    /// circles to always appear as circles instead of ellipses.
    /// </summary>
    public bool ManageAxisLimits { get => PolarAxis.ManageAxisLimits; set => PolarAxis.ManageAxisLimits = value; }

    public virtual void UpdateAxisLimits(Plot plot) => PolarAxis.UpdateAxisLimits(plot);

    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// If enabled, the Polar axis will be drawn above any <see cref="Series">.
    /// </summary>
    public bool IsAxisAboveData { get; set; } = false;

    public IAxes Axes
    {
        get => PolarAxis.Axes;
        set => PolarAxis.Axes = value;
    }

    public IEnumerable<LegendItem> LegendItems => Series.Select(s => new LegendItem
    {
        LabelText = s.LegendText,
        FillStyle = s.FillStyle,
        LineStyle = s.LineStyle,
    });

    public AxisLimits GetAxisLimits() => PolarAxis.GetAxisLimits();

    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
            return;

        if (Series.Count == 0)
            return;

        using SKPaint paint = new();

        // Don’t render axis yet if it’s supposed to be above the data
        if (!IsAxisAboveData)
            PolarAxis.Render(rp);

        for (int i = 0; i < Series.Count; i++)
        {
            Coordinates[] cs1 = PolarAxis.GetCoordinates(Series[i].Values, clockwise: true);
            Pixel[] pixels = cs1.Select(Axes.GetPixel).ToArray();
            Drawing.FillPath(rp.Canvas, paint, pixels, Series[i].FillStyle);
            Drawing.DrawPath(rp.Canvas, paint, pixels, Series[i].LineStyle, close: true);
        }

        if (IsAxisAboveData)
            PolarAxis.Render(rp);
    }
}
