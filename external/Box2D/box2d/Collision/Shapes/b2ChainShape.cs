﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.Common;

namespace Box2D.Collision.Shapes
{
    public class b2ChainShape : b2Shape
    {
        /// The vertices. Owned by this class.
        protected b2Vec2[] m_vertices;

        /// The vertex count.
        protected int m_count;

        protected b2Vec2 m_prevVertex = new b2Vec2();
        protected b2Vec2 m_nextVertex = new b2Vec2();
        protected bool m_hasPrevVertex, m_hasNextVertex;

        public b2ChainShape()
        {
            m_type = b2ShapeType.e_chain;
            m_radius = b2Settings.b2_polygonRadius;
            m_vertices = null;
            m_count = 0;
            m_hasPrevVertex = false;
            m_hasNextVertex = false;
        }


        public virtual void CreateLoop(b2Vec2[] vertices, int count)
        {
            m_count = count + 1;
            m_vertices = new b2Vec2[m_count];
            Array.Copy(vertices, m_vertices, count);
            m_vertices[count] = m_vertices[0];
            m_prevVertex = m_vertices[m_count - 2];
            m_nextVertex = m_vertices[1];
            m_hasPrevVertex = true;
            m_hasNextVertex = true;
        }

        public virtual void CreateChain(b2Vec2[] vertices, int count)
        {
            m_count = count;
            m_vertices = new b2Vec2[count];
            Array.Copy(m_vertices, vertices, count);
            m_hasPrevVertex = false;
            m_hasNextVertex = false;
        }

        public virtual void SetPrevVertex(b2Vec2 prevVertex)
        {
            m_prevVertex = prevVertex;
            m_hasPrevVertex = true;
        }

        public virtual void SetNextVertex(b2Vec2 nextVertex)
        {
            m_nextVertex = nextVertex;
            m_hasNextVertex = true;
        }

        public b2ChainShape(b2ChainShape clone)
            : base((b2Shape)clone)
        {
            CreateChain(clone.m_vertices, m_count);
            m_prevVertex = clone.m_prevVertex;
            m_nextVertex = clone.m_nextVertex;
            m_hasPrevVertex = clone.m_hasPrevVertex;
            m_hasNextVertex = clone.m_hasNextVertex;
        }

        public virtual b2Shape Clone()
        {
            b2ChainShape clone = new b2ChainShape(this);
            return clone;
        }

        public virtual int GetChildCount()
        {
            // edge count = vertex count - 1
            return m_count - 1;
        }

        public virtual b2EdgeShape GetChildEdge(int index)
        {
            b2EdgeShape edge = new b2EdgeShape();
            edge.ShapeType = b2ShapeType.e_edge;
            edge.Radius = m_radius;

            edge.Vertex1 = m_vertices[index + 0];
            edge.Vertex2 = m_vertices[index + 1];

            if (index > 0)
            {
                edge.Vertex0 = m_vertices[index - 1];
                edge.HasVertex0 = true;
            }
            else
            {
                edge.Vertex0 = m_prevVertex;
                edge.HasVertex0 = m_hasPrevVertex;
            }

            if (index < m_count - 2)
            {
                edge.Vertex3 = m_vertices[index + 2];
                edge.HasVertex3 = true;
            }
            else
            {
                edge.Vertex3 = m_nextVertex;
                edge.HasVertex3 = m_hasNextVertex;
            }
            return (edge);
        }

        public virtual bool TestPoint(b2Transform xf, b2Vec2 p)
        {
            return false;
        }

        public virtual bool RayCast(out b2RayCastOutput output, b2RayCastInput input,
                                    b2Transform xf, int childIndex)
        {
            b2EdgeShape edgeShape = new b2EdgeShape();
            output = b2RayCastOutput.Zero;

            int i1 = childIndex;
            int i2 = childIndex + 1;
            if (i2 == m_count)
            {
                i2 = 0;
            }

            edgeShape.Vertex1 = m_vertices[i1];
            edgeShape.Vertex2 = m_vertices[i2];

            b2RayCastOutput co = b2RayCastOutput.Zero;
            bool b = edgeShape.RayCast(out co, input, xf, 0);
            output = co;
            return (b);
        }

        public virtual b2AABB ComputeAABB(b2Transform xf, int childIndex)
        {
            int i1 = childIndex;
            int i2 = childIndex + 1;
            if (i2 == m_count)
            {
                i2 = 0;
            }

            b2Vec2 v1 = b2Math.b2Mul(xf, m_vertices[i1]);
            b2Vec2 v2 = b2Math.b2Mul(xf, m_vertices[i2]);

            b2AABB aabb = new b2AABB();
            aabb.lowerBound = b2Math.b2Min(v1, v2);
            aabb.upperBound = b2Math.b2Max(v1, v2);
            return (aabb);
        }

        public virtual b2MassData ComputeMass(float density)
        {
            b2MassData massData = new b2MassData();
            massData.mass = 0.0f;
            massData.center.SetZero();
            massData.I = 0.0f;
            return (massData);
        }

    }
}
