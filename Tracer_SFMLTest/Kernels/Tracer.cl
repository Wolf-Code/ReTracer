__kernel void test( __global int* Out )
{
	int x = get_global_id( 0 );

	Out[ x ] = 4;
}