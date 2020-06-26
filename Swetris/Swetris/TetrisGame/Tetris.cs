using Swetris.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Urho;
using Xamarin.Essentials;

namespace Swetris.TetrisGame
{
    public class Tetris : Application
    {
        public static Tetris T;

        private Node[,] blocks = new Node[10,20];
        private (Node n, byte x, byte y)[] flying = new (Node n, byte x, byte y)[4];
        #region ctor
        public Tetris(ApplicationOptions options) : base(options)
        {
        }

        public Tetris(IntPtr handle) : base(handle)
        {
        }

        protected Tetris(UrhoObjectFlag emptyFlag) : base(emptyFlag)
        {
        }
        #endregion

        private Random r = new Random();
        private Color[] colors = new Color[]
        {
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Magenta,
            Color.Red,
            Color.Cyan,
            Color.White
        };

        private List<(byte x, byte y)[]> presets = new List<(byte, byte)[]>
        {
            // T
            new (byte, byte)[]
            {
                (5, 18),
                (5, 19),
                (4, 18),
                (6, 18),
            },
            // I
            new (byte, byte)[]
            {
                (6, 19),
                (4, 19),
                (5, 19),
                (7, 19),
            },
            // L
            new (byte, byte)[]
            {
                (5, 19),
                (4, 19),
                (6, 19),
                (4, 18),
            },
            // ⅃
            new (byte, byte)[]
            {
                (5, 19),
                (4, 19),
                (6, 19),
                (6, 18),
            },
            // _|‾
            new (byte, byte)[]
            {
                (5, 19),
                (4, 18),
                (5, 18),
                (6, 19),
            },
            // ‾|_
            new (byte, byte)[]
            {
                (5, 19),
                (4, 19),
                (5, 18),
                (6, 18),
            },
            // O
            new (byte, byte)[]
            {
                (5, 18),
                (5, 19),
                (6, 18),
                (6, 19),
            },
        };

        private void PushDown()
        {
            while(FlyingTick()) { }
        }

        private void RotateLeft()
        {
            (byte x, byte y)[] arr = new (byte x, byte y)[4];

            var x_1 = flying[0].x;
            var y_1 = flying[0].y;
            for (byte b = 0; b < 4; b++)
            {
                var x = flying[b].x;
                var y = flying[b].y;
                var x_2 = (byte)(x - (x - x_1) - (y - y_1));
                var y_2 = (byte)(y + (x - x_1) - (y - y_1));
                if (x_2 < 0 || x_2 > 9 || y_2 < 0 || y_2 > 19)
                    return;
                arr[b] = (x_2, y_2);
            }

            for (byte b = 0; b < 4; b++)
            {
                flying[b].x = arr[b].x;
                flying[b].y = arr[b].y;
            }    

            for (byte b = 0; b < 4; b++)
                UpdateBoxPos(flying[b].n, flying[b].x, flying[b].y);
        }
        private void RotateRight()
        {
            for (int i = 0; i < 3; i++)
                RotateLeft();
        }

        private bool SetFlyingBlock()
        {
            var rand = r.Next(presets.Count);
            var b = presets[rand];
            for (byte i = 0; i < 4; i++)
            {
                if (blocks[b[i].x, b[i].y] != null)
                    return false;
            }

            for (byte i = 0; i < 4; i++)
            {
                var box = AddBox(colors[rand]);
                UpdateBoxPos(box, b[i].x, b[i].y);
                flying[i] = (box, b[i].x, b[i].y);
            }
            return true;
        }

        private void SolidifyFlying()
        {
            foreach (var block in flying)
            {
                blocks[block.x, block.y] = block.n;
            }

            if (!SetFlyingBlock())
            {
                // Game lost
                scene.UpdateEnabled = false;
            }
            var lines = DestroyLines();
            GameViewModel.Game.Score += lines switch
            {
                1 => 25,
                2 => 50,
                3 => 100,
                4 => 200,
                _ => 0,
            };
        }

        private int DestroyLines()
        {
            var destroyed = 0;
            for (var y = 0; y < 20; y++)
            {
                bool allFilled = true;
                for (var x = 0; x < 10 && allFilled; x++)
                {
                    if(blocks[x, y] == null)
                    {
                        allFilled = false;
                    }
                }
                if (allFilled)
                {
                    destroyed++;
                    for (byte x = 0; x < 10; x++)
                    {
                        scene.RemoveChild(blocks[x, y]);
                        for (var y_1 = y; y_1 < 19; y_1++)
                        {
                            blocks[x, y_1] = blocks[x, y_1 + 1];
                            if (blocks[x, y_1] != null)
                                UpdateBoxPos(blocks[x, y_1], x, (byte)y_1);
                        }
                        if (blocks[x, 19] != null)
                        {
                            scene.RemoveChild(blocks[x, 19]);
                            blocks[x, 19] = null;
                        }
                    }
                    y--;
                }
            }
            return destroyed;
        }

        protected override void Start()
        {
            base.Start();
            T = this;
            var scene = Create3DObject();
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            if(!Accelerometer.IsMonitoring)
                Accelerometer.Start(SensorSpeed.Game);
            Input.TouchBegin += (e) =>
            {
                rotated = false;
                
            };
            Input.TouchMove += (e) => {
                if (!scene.UpdateEnabled) return;
                if (e.DX < -20 && !rotated && e.DY < 10 && e.DY > -10)
                {
                    RotateLeft();
                    rotated = true;
                }
                if (e.DX > 20 && !rotated && e.DY < 10 && e.DY > -10)
                {
                    RotateRight();
                    rotated = true;
                }
                if (e.DX < 10 && e.DX > -10 && !rotated && e.DY > 20)
                {
                    PushDown();
                    rotated = true;
                }
            };
            Tetris.scene = scene;
            SetFlyingBlock();
        }

        private bool rotated = false;

        DateTime lastEvent;
        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            if ((DateTime.Now - lastEvent).TotalSeconds > .25)
            {
                if (e.Reading.Acceleration.X < -1.3f)
                {
                    lastEvent = DateTime.Now;
                    Debug.WriteLine("left");
                    if (flying.Any(p => p.x == 0 || blocks[p.x-1, p.y] != null)) return;

                    for (byte i = 0; i < 4; i++)
                    {
                        flying[i].x--;
                        UpdateBoxPos(flying[i].n, flying[i].x, flying[i].y);
                    }
                }
                if (e.Reading.Acceleration.X > 1.5f)
                {
                    lastEvent = DateTime.Now;
                    Debug.WriteLine("right");
                    if (flying.Any(p => p.x == 9 || blocks[p.x+1, p.y] != null)) return;
                    for (byte i = 0; i < 4; i++)
                    {
                        flying[i].x++;
                        UpdateBoxPos(flying[i].n, flying[i].x, flying[i].y);
                    }
                }
            }
        }

        private bool FlyingTick()
        {
            if (flying == null) return false;
            if (flying.Any(p=>p.y == 0) || flying.Any(p=>blocks[p.x, p.y - 1] != null))
            {
                // Block hit
                SolidifyFlying();
                return false;
            }
            else
            {
                // Space empty
                for (byte i = 0; i < 4; i++)
                {
                    flying[i].y--;
                    UpdateBoxPos(flying[i].n, flying[i].x, flying[i].y);
                }
                return true;
            }
        }

        float Speed => 1f*(float)Math.Exp((-GameViewModel.Game.Score/100) / 10f) + 0.25f;
        float time = 0;
        protected override void OnUpdate(float timeStep)
        {
            if (!scene.UpdateEnabled) return;
            time += timeStep;
            if(time > Speed)
            {
                time -= Speed;
                FlyingTick();
            }
            base.OnUpdate(timeStep);
        }

        float leftShift = 0;
        #region 3D

        private Camera cam;
        private Scene Create3DObject()
        {
            var scene = new Scene();
            scene.CreateComponent<Octree>();

            var planeNode = scene.CreateChild("Plane");
            planeNode.Scale = new Vector3(3, 0, 1f);
            planeNode.Position = new Vector3(0f+ leftShift, -3.2f, 9);
            var planeObject = planeNode.CreateComponent<StaticModel>();
            planeObject.Model = ResourceCache.GetModel("Models/Plane.mdl");

            var planeNode2 = scene.CreateChild("Plane2");
            planeNode2.Scale = new Vector3(6, 0, 1f);
            planeNode2.Rotation = new Quaternion(0, 0, -90);
            planeNode2.Position = new Vector3(-1.5f+ leftShift, -0.2f, 9);
            var planeObject2 = planeNode2.CreateComponent<StaticModel>();
            planeObject2.Model = ResourceCache.GetModel("Models/Plane.mdl");

            var planeNode3 = scene.CreateChild("Plane3");
            planeNode3.Scale = new Vector3(6, 0, 1f);
            planeNode3.Rotation = new Quaternion(0, 0, +90);
            planeNode3.Position = new Vector3(+1.5f+ leftShift, -0.2f, 9);
            var planeObject3 = planeNode3.CreateComponent<StaticModel>();
            planeObject3.Model = ResourceCache.GetModel("Models/Plane.mdl");

            /*var planeNode4 = scene.CreateChild("Plane4");
            planeNode4.Scale    = new Vector3(3, 0, 6);
            planeNode4.Rotation = new Quaternion(-90, 0, 0);
            planeNode4.Position = new Vector3(0f, -.2f, 9.15f);
            var planeObject4 = planeNode4.CreateComponent<StaticModel>();
            planeObject4.Model = ResourceCache.GetModel("Models/Plane.mdl");*/

            Node lightNode = scene.CreateChild(name: "light");
            lightNode.SetDirection(new Vector3(0f, 90f, 4f));
            lightNode.CreateComponent<Light>();

            Node cameraNode = scene.CreateChild(name: "camera");
            cameraNode.Position = new Vector3(0, 0, 0);
            cam = cameraNode.CreateComponent<Camera>();
            Renderer.SetViewport(0, new Viewport(scene, cam, null));
            return scene;
        }

        private static Scene scene;
        private Node AddBox(Color color)
        {
            var boxNode = new Node();
            boxNode.Rotation = new Quaternion(0, 0, 0, 0);
            boxNode.SetScale(.28f);
            StaticModel modelObject = boxNode.CreateComponent<StaticModel>();
            modelObject.Model = ResourceCache.GetModel("Models/Box.mdl");
            modelObject.SetMaterial(Material.FromColor(color));
            scene.AddChild(boxNode);
            return boxNode;
        }
        private void UpdateBoxPos(Node boxNode, byte x, byte y)
        {
            boxNode.Position = new Vector3(-1.35f+x*.3f + leftShift, -3+y*.3f, 9);
        }
        #endregion 3D
    }
}
