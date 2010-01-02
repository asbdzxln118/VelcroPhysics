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

using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FarseerPhysics.TestBed.Tests
{
    public class ConfinedTest : Test
    {
        private const int ColumnCount = 0;
        private const int RowCount = 0;

        private ConfinedTest()
        {
            {
                
                Body ground = _world.CreateBody();


                // Floor
                Vertices edge = PolygonTools.CreateEdge(new Vector2(-10.0f, 0.0f), new Vector2(10.0f, 0.0f));
                PolygonShape shape = new PolygonShape(edge, 0);

                ground.CreateFixture(shape);

                // Left wall
                shape.Set(PolygonTools.CreateEdge(new Vector2(-10.0f, 0.0f), new Vector2(-10.0f, 20.0f)));
                ground.CreateFixture(shape);

                // Right wall
                shape.Set(PolygonTools.CreateEdge(new Vector2(10.0f, 0.0f), new Vector2(10.0f, 20.0f)));
                ground.CreateFixture(shape);

                // Roof
                shape.Set(PolygonTools.CreateEdge(new Vector2(-10.0f, 20.0f), new Vector2(10.0f, 20.0f)));
                ground.CreateFixture(shape);
            }

            const float radius = 0.5f;
            CircleShape shape2 = new CircleShape(radius, 1);
            shape2.Position = Vector2.Zero;

            for (int j = 0; j < ColumnCount; ++j)
            {
                for (int i = 0; i < RowCount; ++i)
                {
                    Body body = _world.CreateBody();
                    body.BodyType = BodyType.Dynamic;
                    body.Position = new Vector2(-10.0f + (2.1f * j + 1.0f + 0.01f * i) * radius, (2.0f * i + 1.0f) * radius);

                    Fixture fixture = body.CreateFixture(shape2);
                    fixture.Friction = 0.1f;
                }
            }

            _world.Gravity = Vector2.Zero;
        }

        private void CreateCircle()
        {
            const float radius = 0.5f;
            CircleShape shape = new CircleShape(radius, 1);
            shape.Position = Vector2.Zero;

            Body body = _world.CreateBody();
            body.BodyType = BodyType.Dynamic;
            body.Position = new Vector2(Rand.RandomFloat(), (2.0f + Rand.RandomFloat()) * radius);

            Fixture fixture = body.CreateFixture(shape);
            fixture.Friction = 0;
        }

        public override void Keyboard(KeyboardState state, KeyboardState oldState)
        {
            if (state.IsKeyDown(Keys.C))
            {
                CreateCircle();
            }
        }

        public override void Step(Framework.Settings settings)
        {
            uint oldFlag = settings.enableContinuous;

            settings.enableContinuous = 0;
            base.Step(settings);
            _debugView.DrawString(5, _textLine, "Press 'c' to create a circle.");
            _textLine += 15;

            settings.enableContinuous = oldFlag;
        }

        internal static Test Create()
        {
            return new ConfinedTest();
        }
    }
}