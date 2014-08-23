using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 2-tuple
/// </summary>
public struct UKTuple<A,B>
{
	public A a;
	public B b;

	public A x { get { return a; } }
	public B y { get { return b; } }

	public UKTuple(A a, B b)
	{
		this.a = a;
		this.b = b;
	}

	public override string ToString ()
	{
		return string.Format ("<{0},{1}>", a,b);
	}
}

/// <summary>
/// 3-tuple
/// </summary>
public struct UKTuple<A,B,C>
{
	public A a;
	public B b;
	public C c;

	public A x { get { return a; } }
	public B y { get { return b; } }
	public C z { get { return c; } }

	public UKTuple(A a, B b, C c)
	{
		this.a = a;
		this.b = b;
		this.c = c;
	}
	
	public override string ToString ()
	{
		return string.Format ("<{0},{1},{2}>", a,b,c);
	}
}

/// <summary>
/// 4-tuple
/// </summary>
public struct UKTuple<A,B,C,D>
{
	public A a;
	public B b;
	public C c;
	public D d;
	
	public UKTuple(A a, B b, C c, D d)
	{
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}
	
	public override string ToString ()
	{
		return string.Format ("<{0},{1},{2},{3}>", a,b,c,d);
	}
}
