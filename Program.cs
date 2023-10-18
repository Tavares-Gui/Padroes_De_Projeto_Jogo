using System;
using System.Drawing;
using Pamella;

App.Open<Joguin>();

public class Enemy
{
	public State State { get; set; }
	public float X { get; set; }
	public float Y { get; set; }
	public float Angle { get; set; }

    public void Act()
    {
        this.State.Act();
    }
}

public abstract class State
{
	public Enemy Enemy { get; set; }

	public State NextState { get; set; }

    public abstract void Act();
}

public class MovingState : State
{
    public float XTarget { get; set; }
    public float YTarget { get; set; }


    public override void Act()
    {
        var dx = XTarget - Enemy.X;
        var dy = YTarget - Enemy.Y; 

        var mod = MathF.Sqrt(dx * dx + dy * dy);

        if(mod < 5)
		{
            this.Enemy.State = NextState;
			return;
		}

        Enemy.X += 100 * dx / mod / 40;
        Enemy.Y += 100 * dy / mod / 40;
    }
}

public class RotateState : State
{
    public float AngleTarget { get; set; }


    public override void Act()
    {
        var dTheta = AngleTarget - Enemy.Angle;

        if(MathF.Abs(dTheta) < 0.05)
		{
            this.Enemy.State = NextState;
			return;
		}

        Enemy.Angle += 0.1f * Math.Sign(dTheta);
    }
}

public class Joguin : View
{
	Enemy enemy1;

	protected override void OnStart(IGraphics g)
	{	
		enemy1 = new Enemy();
		enemy1.X = 200;
		enemy1.Y = 200;

		var builder = new StateBuilder();

		builder
			.SetEnemy(enemy1)
			.AddMovingState(400, 200)
			.AddRotateState(MathF.PI / 2)
			.AddMovingState(400, 600)
			.AddRotateState(3 * MathF.PI / 2)
			.Build();
	
		g.SubscribeKeyDownEvent(Key =>
		{
			if(Key == Input.Escape)
			{
				App.Close();
			}
		});
		AlwaysInvalidateMode();
	}

    protected override void OnFrame(IGraphics g)
	{
		enemy1.Act();
	}

	protected override void OnRender(IGraphics g)
	{
		g.Clear(Color.DarkGreen);

		var cos = MathF.Cos(enemy1.Angle);
		var sin = MathF.Sin(enemy1.Angle);

		g.FillPolygon(
			new PointF[] {
				new PointF(enemy1.X, enemy1.Y),
				new PointF(
					enemy1.X + 250 * cos - 50 * sin,
					enemy1.Y + 250 * sin + 50 * cos
				),
				new PointF(
					enemy1.X + 250 * cos + 50 * sin,
					enemy1.Y + 250 * sin - 50 * cos
				),
			}, Brushes.Yellow
		);

		g.FillRectangle(
			enemy1.X - 15, enemy1.Y - 15,
			30,30, Brushes.Red
		);
	}
}
