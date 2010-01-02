﻿/*
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerPhysics.TestBed.Tests
{
    public class RayCastTest : Test
    {
        private const int MaxBodies = 256;

        private RayCastTest()
        {
            // Ground body
            {
                
                Body ground = _world.CreateBody();

                PolygonShape shape = new PolygonShape(0);
                shape.SetAsEdge(new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f));
                ground.CreateFixture(shape);
            }

            {
                Vertices vertices = new Vertices(3);
                vertices[0] = new Vector2(-0.5f, 0.0f);
                vertices[1] = new Vector2(0.5f, 0.0f);
                vertices[2] = new Vector2(0.0f, 1.5f);
                _polygons[0] = new PolygonShape(0);
                _polygons[0].Set(vertices);
            }

            {
                Vertices vertices2 = new Vertices(3);
                vertices2[0] = new Vector2(-0.1f, 0.0f);
                vertices2[1] = new Vector2(0.1f, 0.0f);
                vertices2[2] = new Vector2(0.0f, 1.5f);
                _polygons[1] = new PolygonShape(0);
                _polygons[1].Set(vertices2);
            }

            {
                const float w = 1.0f;
                float b = w / (2.0f + (float)Math.Sqrt(2.0));
                float s = (float)Math.Sqrt(2.0) * b;

                Vertices vertices3 = new Vertices(8);
                vertices3[0] = new Vector2(0.5f * s, 0.0f);
                vertices3[1] = new Vector2(0.5f * w, b);
                vertices3[2] = new Vector2(0.5f * w, b + s);
                vertices3[3] = new Vector2(0.5f * s, w);
                vertices3[4] = new Vector2(-0.5f * s, w);
                vertices3[5] = new Vector2(-0.5f * w, b + s);
                vertices3[6] = new Vector2(-0.5f * w, b);
                vertices3[7] = new Vector2(-0.5f * s, 0.0f);
                _polygons[2] = new PolygonShape(0);
                _polygons[2].Set(vertices3);
            }

            {
                _polygons[3] = new PolygonShape(0);
                _polygons[3].SetAsBox(0.5f, 0.5f);
            }

            {
                _circle = new CircleShape(0.5f, 0);
            }

            _bodyIndex = 0;

            _angle = 0.0f;
        }

        public override void Keyboard(KeyboardState state, KeyboardState oldState)
        {
            if (state.IsKeyDown(Keys.D1) && oldState.IsKeyUp(Keys.D1))
            {
                Create(0);
            }
            if (state.IsKeyDown(Keys.D2) && oldState.IsKeyUp(Keys.D2))
            {
                Create(1);
            }
            if (state.IsKeyDown(Keys.D3) && oldState.IsKeyUp(Keys.D3))
            {
                Create(2);
            }
            if (state.IsKeyDown(Keys.D4) && oldState.IsKeyUp(Keys.D4))
            {
                Create(3);
            }
            if (state.IsKeyDown(Keys.D5) && oldState.IsKeyUp(Keys.D5))
            {
                Create(4);
            }
            if (state.IsKeyDown(Keys.D) && oldState.IsKeyUp(Keys.D))
            {
                DestroyBody();
            }
        }

        private void DestroyBody()
        {
            for (int i = 0; i < MaxBodies; ++i)
            {
                if (_bodies[i] != null)
                {
                    _world.DestroyBody(_bodies[i]);
                    _bodies[i] = null;
                    return;
                }
            }
        }

        public override void Step(Framework.Settings settings)
        {
            base.Step(settings);
            _debugView.DrawString(5, _textLine, "Press 1-5 to drop stuff");
            _textLine += 15;

            const float L = 11.0f;
            Vector2 point1 = new Vector2(0.0f, 10.0f);
            Vector2 d = new Vector2(L * (float)Math.Cos(_angle), L * (float)Math.Sin(_angle));
            Vector2 point2 = point1 + d;


            Fixture fixture = null;
            Vector2 point = Vector2.Zero, normal = Vector2.Zero;
            _world.RayCast((f, p, n, fr) =>
                               {
                                   fixture = f;
                                   point = p;
                                   normal = n;
                                   return fr;
                               }, point1, point2);

            if (fixture != null)
            {
                _debugView.DrawPoint(point, .5f, new Color(0.4f, 0.9f, 0.4f));

                _debugView.DrawSegment(point1, point, new Color(0.8f, 0.8f, 0.8f));

                Vector2 head = point + 0.5f * normal;
                _debugView.DrawSegment(point, head, new Color(0.9f, 0.9f, 0.4f));
            }
            else
            {
                _debugView.DrawSegment(point1, point2, new Color(0.8f, 0.8f, 0.8f));
            }

            _angle += 0.25f * Settings.Pi / 180.0f;
        }

        private void Create(int index)
        {
            if (_bodies[_bodyIndex] != null)
            {
                _world.DestroyBody(_bodies[_bodyIndex]);
                _bodies[_bodyIndex] = null;
            }

            float x = Rand.RandomFloat(-10.0f, 10.0f);
            float y = Rand.RandomFloat(0.0f, 20.0f);

            _bodies[_bodyIndex] = _world.CreateBody();

            _bodies[_bodyIndex].Position = new Vector2(x, y);
            _bodies[_bodyIndex].Rotation = Rand.RandomFloat(-Settings.Pi, Settings.Pi);

            if (index == 4)
            {
                _bodies[_bodyIndex].AngularDamping = 0.02f;
            }

            if (index < 4)
            {
                Fixture fixture = _bodies[_bodyIndex].CreateFixture(_polygons[index]);
                fixture.Friction = 0.3f;
            }
            else
            {
                Fixture fixture = _bodies[_bodyIndex].CreateFixture(_circle);
                fixture.Friction = 0.3f;
            }

            _bodyIndex = (_bodyIndex + 1) % MaxBodies;
        }

        internal static Test Create()
        {
            return new RayCastTest();
        }

        private int _bodyIndex;
        private Body[] _bodies = new Body[MaxBodies];
        private PolygonShape[] _polygons = new PolygonShape[4];
        private CircleShape _circle;

        private float _angle;
    }
}