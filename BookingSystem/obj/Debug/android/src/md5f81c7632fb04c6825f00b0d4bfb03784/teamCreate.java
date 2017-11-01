package md5f81c7632fb04c6825f00b0d4bfb03784;


public class teamCreate
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("BookingSystem.Activities.teamCreate, BookingSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", teamCreate.class, __md_methods);
	}


	public teamCreate () throws java.lang.Throwable
	{
		super ();
		if (getClass () == teamCreate.class)
			mono.android.TypeManager.Activate ("BookingSystem.Activities.teamCreate, BookingSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
