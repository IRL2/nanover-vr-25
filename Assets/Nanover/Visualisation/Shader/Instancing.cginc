/// Contains methods for using instancing to access members of various arrays.

#ifndef INSTANCING_CGINC_INCLUDED

    #define INSTANCING_CGINC_INCLUDED
    
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    
        #if defined(FILTER_ARRAY)
            StructuredBuffer<int> FilterArray;
            #define instance_id FilterArray[unity_InstanceID]
        #else
            #define instance_id unity_InstanceID
        #endif
    #else
        #define instance_id 0
    #endif

    /// Position array, representing 3D points.

    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && defined(POSITION_ARRAY)

        StructuredBuffer<float3> PositionArray;

        float3 instance_position() {
            return PositionArray[instance_id];
        }

    #else

        float3 instance_position() {
            return float3(0,0,0);
        }

    #endif

    /// Color array

    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && defined(COLOR_ARRAY)

        StructuredBuffer<float4> ColorArray;

        float4 instance_color() {
            return pow(ColorArray[instance_id], 2.2);
        }

    #else

        float4 instance_color() {
            return float4(1,1,1,1);
        }

    #endif


    /// Scale array, representing homogenous scaling

    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && defined(SCALE_ARRAY)

        StructuredBuffer<float> ScaleArray : register(t3);

        float instance_scale() {
            return ScaleArray[instance_id];
        }

    #else

        float instance_scale() {
            return 1;
        }

    #endif

    /// Edge array, representing connections between particles

    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && defined(EDGE_ARRAY)

        struct Edge {
            int a;
            int b;
        };

        StructuredBuffer<Edge> EdgeArray;

        Edge instance_edge() {
            return EdgeArray[instance_id];
        }

        #if defined(POSITION_ARRAY)

            float3 edge_position(float t) {
                Edge edge = instance_edge();
                return lerp(PositionArray[edge.a], PositionArray[edge.b], t);
            }

        #endif

        #if defined(COLOR_ARRAY)

            float4 edge_color(float t) {
                Edge edge = instance_edge();
                return pow(lerp(ColorArray[edge.a], ColorArray[edge.b], t), 2.2);
            }

        #else

            float4 edge_color(float t) {
                return float4(1,1,1,1);
            }

        #endif

        #if defined(SCALE_ARRAY)

            float edge_scale(float t) {
                Edge edge = instance_edge();
                return lerp(ScaleArray[edge.a], ScaleArray[edge.b], t);
            }

        #else

            float edge_scale(float t) {
                return 1;
            }

        #endif

    #else

        float3 edge_position(float t) {
            return float3(0,0,0);
        }

        float4 edge_color(float t) {
            return float4(1,1,1,1);
        }

        float edge_scale(float t) {
            return 1;
        }

    #endif
    
    /// Edge count array

    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) && defined(COLOR_ARRAY)

        StructuredBuffer<int> EdgeCountArray;

        int instance_edge_count() {
            return EdgeCountArray[instance_id];
        }

    #else

        int instance_edge_count() {
            return 1;
        }

    #endif

#endif
