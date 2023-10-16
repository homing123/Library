using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace Define
{
    public struct Circle2D
    {
        public Vector2 Center;
        public float Radius;

        public Circle2D(Vector2 center, Vector2 other_point)
        {
            Center = center;
            Radius = Vector2.Distance(center, other_point);
        }
        public Circle2D(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }

    public struct Line2D
    {
        public Vector2 Direction;
        public Vector2 Point;
        float Gradient;
        public Line2D(Vector2 point, Vector2 dir)
        {
            if(dir.x < 0)
            {
                dir.x *= -1;
                dir.y *= -1;
            }
            else if(dir.x == 0 && dir.y < 0)
            {
                dir.y *= -1;
            }
            Direction = dir.normalized;
            Point = point;

            if (Direction.x == 0)
            {
                Gradient = 0;
            }
            else
            {
                Gradient = Direction.y / Direction.x;
            }
        }

        public float Get_Gradient(out bool isInfinity)
        {
            if(Direction.x != 0)
            {
                isInfinity = false;
                return Gradient;
            }
            else
            {
                isInfinity = true;
                return 0;
            }
        }
        public float Get_Point_y(float x, out bool isNone)
        {
            isNone = false;
            if (Direction.x == 0)
            {
                if (Point.x != x)
                {
                    isNone = true;
                    return 0;
                    //throw new Exception("Line gradient x == 0 and point x != x. point x : " + Point.x + " x : " + x);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if(Direction.y == 0)
                {
                    return Point.y;
                }
                else
                {
                    return Point.y + (Direction.y * (x - Point.x) / Direction.x);
                }
            }
        }
        public float Get_Point_x(float y, out bool isNone)
        {
            isNone = false;
            if (Direction.y == 0)
            {
                if (Point.y != y)
                {
                    isNone = true;
                    return 0;
                    //throw new Exception("Line gradient y == 0 and point y != y. point y : " + Point.y + " y : " + y);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (Direction.x == 0)
                {
                    return Point.x;
                }
                else
                {
                    return Point.x + (Direction.x * (y - Point.y) / Direction.y);
                }
            }
        }
        public Vector2 Get_PointofIntersection(Line2D other_line, out bool isNone)
        {
            if(Direction == other_line.Direction)
            {
                bool isnone = false;
                Vector2 other_point;
                other_point.x = Point.x;
                other_point.y = other_line.Get_Point_y(Point.x, out isnone);
                if (isnone == false && Point == other_point)
                {
                    isNone = false;
                    return Point;
                }
                else 
                {
                    isNone = true;
                    return Vector2.zero;
                }
            }
            else
            {
                isNone = false;
                bool isInfinity_1, isInfinity_2;
                float grad_1 = Get_Gradient(out isInfinity_1);
                float grad_2 = Get_Gradient(out isInfinity_2);
                if(isInfinity_1== true)
                {
                    bool isnone;
                    float y = other_line.Get_Point_y(Point.x, out isnone);
                    if(isnone == false)
                    {
                        return new Vector2(Point.x, y);
                    }
                    else
                    {
                        isNone = true;
                        return Vector2.zero;
                    }
                }
                else if(isInfinity_2 == true)
                {
                    bool isnone;
                    float y = Get_Point_y(other_line.Point.x, out isnone);
                    if (isnone == false)
                    {
                        return new Vector2(other_line.Point.x, y);
                    }
                    else
                    {
                        isNone = true;
                        return Vector2.zero;
                    }
                }
                else
                {
                    float x = (grad_1 * Point.x - Point.y - grad_2 * other_line.Point.x + other_line.Point.y) / (grad_1 - grad_2);
                    bool temp;
                    float y = Get_Point_y(x, out temp);
                    return new Vector2(x, y);
                }
            }
        }
    }
    public class Math_Define
    {


        public static (Vector2 Contact_Pos, float Distance) Get_Distance_Line_Pos_2D(float x_coe, float y_coe, float num_coe, Vector2 point)
        {
            //평면 점 point, 선 ax + by + c = 0 (a = x_coe, b = y_coe, c = num_coe) 
            //Contact_Pos = 수선의발
            //점과 선의 거리 및 수선의발을 구하는 함수

            Vector2 contact_pos = default;
            if (x_coe == 0 && y_coe == 0)
            {
                throw new Exception("Get_Distance_Line_Pos_2D coefficient error : " + x_coe + " " + y_coe);
            }
            if (x_coe == 0)
            {
                contact_pos.x = point.x;
                contact_pos.y = -num_coe / y_coe;
            }
            else if (y_coe == 0)
            {
                contact_pos.y = point.y;
                contact_pos.y = -num_coe / x_coe;
            }
            else
            {
                contact_pos.x = (-point.y * x_coe * y_coe + y_coe * y_coe * point.x - x_coe * num_coe) / (x_coe * x_coe + y_coe * y_coe);
                contact_pos.y = -(x_coe * contact_pos.x + num_coe) / y_coe;
            }
            return (contact_pos, Vector2.Distance(point, contact_pos));
        }

        public static Vector2[] Intersection2D_Line_Circle(Circle2D circle, Line2D line)
        {
            //원과 선의 교점
            //원중심을 (0,0)으로 옮겨서 계산

            Circle2D temp_circle = new Circle2D(Vector2.zero, circle.Radius);
            Vector2 move_dis = circle.Center;

            Line2D temp_line = new Line2D(line.Point - move_dis, line.Direction);

            bool isInfinity;
            float grad = temp_line.Get_Gradient(out isInfinity);
            if(isInfinity == true || grad >= float.MaxValue || grad <= float.MinValue)
            {
                if(grad> float.MaxValue)
                {
                    Debug.Log(grad + " 최대값 초과");
                }
                else if (grad >= float.MaxValue)
                {
                    Debug.Log(grad + " 최대값 이상");
                }
                else if (grad < float.MinValue)
                {
                    Debug.Log(grad + " 최소값 미만");
                }
                else if (grad <= float.MinValue)
                {
                    Debug.Log(grad + " 최대값 이하");
                }

                //line의 direction.x == 0으로 계산
                bool isNone;
                float intercept_x = temp_line.Get_Point_x(0, out isNone);
                if(isNone == false)
                {
                    if(MathF.Abs(intercept_x) == temp_circle.Radius)
                    {
                        return (new Vector2(intercept_x, 0) + move_dis).ToArray();
                    }
                    else if(MathF.Abs(intercept_x) < temp_circle.Radius)
                    {
                        float y = Mathf.Sqrt(temp_circle.Radius * temp_circle.Radius - intercept_x * intercept_x);
                        return new Vector2[]
                        {
                            new Vector2(intercept_x, y) + move_dis,
                            new Vector2(intercept_x, -y) + move_dis
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                float x_1 = temp_line.Point.x;
                float y_1 = temp_line.Point.y;
                float a = 1 + grad * grad;
                float b = -2 * grad * grad * x_1 + 2 * grad * y_1;
                float c = grad * grad * x_1 * x_1 + y_1 * y_1 * -2 * grad * x_1 * y_1 - temp_circle.Radius * temp_circle.Radius;

                float value = b * b - 4 * a * c;
                if (value < 0)
                {
                    return null;
                }
                else if (value == 0)
                {
                    float x = -b / (2 * a);
                    bool isNone;
                    float y = temp_line.Get_Point_y(x, out isNone);

                    return (new Vector2(x, y) + move_dis).ToArray();
                }
                else
                {
                    float sqrt_value = Mathf.Sqrt(value);
                    float x_2 = (-b + sqrt_value) / (2 * a);
                    float x_3 = (-b - sqrt_value) / (2 * a);
                    bool isNone;

                    float y_2 = temp_line.Get_Point_y(x_2, out isNone);
                    float y_3 = temp_line.Get_Point_y(x_3, out isNone);
                    return new Vector2[]
                    {
                        new Vector2(x_2, y_2) + move_dis,
                        new Vector2(x_3, y_3) + move_dis
                    };
                }
            }
            Debug.Log($"확인 필요.\n CriclePos : {circle.Center}, Radius : {circle.Radius}, line_point : {line.Point}, line_dir : {line.Direction}");
            return null;

        }
        public static Vector2[] Get_Contact_Cricle_Line_2D(Vector2 circle_pos, float radius, float x_coe, float y_coe, float num_coe)
        {

            //선 식 : ax + by + c = 0 (a = x_coe, b = y_coe, c = num_coe) 
            //아래의 a,b,c는 선 식에서의 a,b,c 가 아님
            //원과 선의 접점 구하는 함수

            Vector2[] contact = null;

            if (x_coe == 0 && y_coe == 0)
            {
                throw new Exception("Get_Contact_Cricle_Line_2D coefficient error : " + x_coe + " " + y_coe);
            }
            if (x_coe == 0)
            {
                float y = -num_coe / y_coe;
                float a = 1;
                float b = -2 * circle_pos.x;
                float c = circle_pos.x * circle_pos.x + y * y - 2 * y * circle_pos.y + circle_pos.y * circle_pos.y - radius * radius;

                float value = b * b - 4 * a * c;
                if (value < 0)
                {
                    Debug.Log("0개" + x_coe + " " + y_coe + " " + num_coe + " " + value);
                    return new Vector2[0];
                }
                else if (value == 0)
                {
                    Debug.Log("1개" + x_coe + " " + y_coe + " " + num_coe + " " + value);
                    return new Vector2[1]
                    {
                    new Vector2(-b / (2 * a), y)
                    };
                }
                else
                {
                    return new Vector2[2]
                    {
                    new Vector2((-b + MathF.Sqrt(value)) / (2 * a), y),
                    new Vector2((-b - MathF.Sqrt(value)) / (2 * a), y)
                    };
                }
            }
            else if (y_coe == 0)
            {
                float x = -num_coe / x_coe;
                float a = 1;
                float b = -2 * circle_pos.y;
                float c = x * x - 2 * x * circle_pos.x + circle_pos.x * circle_pos.x + circle_pos.y * circle_pos.y - radius * radius;

                float value = b * b - 4 * a * c;
                if (value < 0)
                {
                    Debug.Log("0개" + x_coe + " " + y_coe + " " + num_coe + " " + value);
                    return new Vector2[0];
                }
                else if (value == 0)
                {
                    Debug.Log("1개" + x_coe + " " + y_coe + " " + num_coe + " " + value);
                    return new Vector2[1]
                    {
                    new Vector2(x, -b / (2 * a))
                    };
                }
                else
                {
                    float value_sqrt = MathF.Sqrt(value);
                    return new Vector2[2]
                    {
                    new Vector2(x, (-b + value_sqrt) / (2 * a)),
                    new Vector2(x, (-b - value_sqrt) / (2 * a))
                    };
                }
            }
            else
            {
                float a = 1 + (x_coe * x_coe) / (y_coe * y_coe);
                float b = -2 * circle_pos.x + 2 * x_coe * num_coe / (y_coe * y_coe) + 2 * circle_pos.y * x_coe / y_coe;
                float c = circle_pos.x * circle_pos.x + circle_pos.y * circle_pos.y - radius * radius + num_coe * num_coe / (y_coe * y_coe) + 2 * circle_pos.y * num_coe / y_coe;
                float value = b * b - 4 * a * c;
                if (value < 0)
                {
                    Debug.Log("0개" + x_coe + " " + y_coe + " " + num_coe + " " + value);
                    return new Vector2[0];
                }
                else if (value == 0)
                {
                    Debug.Log("1개" + x_coe + " " + y_coe + " " + num_coe + " " + value);
                    float x = -b / (2 * a);
                    float y = -(x_coe * x + num_coe) / y_coe;
                    contact = new Vector2[1]
                    {
                    new Vector2(x,y)
                    };
                }
                else
                {
                    float value_sqrt = Mathf.Sqrt(value);
                    float x_1 = (-b + value_sqrt) / (2 * a);
                    float y_1 = -(x_coe * x_1 + num_coe) / y_coe;
                    float x_2 = (-b - value_sqrt) / (2 * a);
                    float y_2 = -(x_coe * x_2 + num_coe) / y_coe;

                    contact = new Vector2[2]
                    {
                    new Vector2(x_1,y_1),
                    new Vector2(x_2,y_2)
                    };
                }
                return contact;
            }
        }
        public static bool isAngleRange(float dir, float angleRange, float angle)
        {
            bool isReverse = false;
            float min = dir - angleRange * 0.5f;
            if (min < 0)
            {
                isReverse = true;
                min += 360;
            }
            float max = dir + angleRange * 0.5f;
            if (max > 360)
            {
                isReverse = true;
                max -= 360;
            }

            if (isReverse)
            {
                if (max >= angle || min <= angle)
                {
                    return true;
                }
            }
            else
            {
                if (min <= angle && max >= angle)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// a각과 b각의 짧은거리
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float AngleDistance(float a, float b)
        {
            float abs = Mathf.Abs(a.ToAngleValue() - b.ToAngleValue());
            return abs > 180 ? 360 - abs : abs;
        }


        #region Sort
        public static float[] Sort_GetValue(float[] arr, E_SortDir dir)
        {
            return _Sort_GetValue(arr, dir);
        }
        public static float[] Sort_GetValue(List<float> list, E_SortDir dir)
        {
            return _Sort_GetValue(list.ToArray(), dir);
        }
        public static int[] Sort_GetValue(int[] arr, E_SortDir dir)
        {
            return _Sort_GetValue(arr, dir);
        }
        public static int[] Sort_GetValue(List<int> list, E_SortDir dir)
        {
            return _Sort_GetValue(list.ToArray(), dir);
        }
        static float[] _Sort_GetValue(float[] arr, E_SortDir dir)
        {
            bool isChanged = false;
            if (dir == E_SortDir.Up)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] > arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;
                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            if (dir == E_SortDir.Down)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] < arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;
                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            return arr;
        }
        static int[] _Sort_GetValue(int[] arr, E_SortDir dir)
        {
            bool isChanged = false;
            if (dir == E_SortDir.Up)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] > arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;
                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            if (dir == E_SortDir.Down)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] < arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;
                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            return arr;
        }
        public static int[] Sort_GetIdx(float[] arr, E_SortDir dir)
        {
            return _Sort_GetIdx(arr, dir);
        }
        public static int[] Sort_GetIdx(List<float> list, E_SortDir dir)
        {
            return _Sort_GetIdx(list.ToArray(), dir);
        }
        public static int[] Sort_GetIdx(int[] arr, E_SortDir dir)
        {
            return _Sort_GetIdx(arr, dir);
        }
        public static int[] Sort_GetIdx(List<int> list, E_SortDir dir)
        {
            return _Sort_GetIdx(list.ToArray(), dir);
        }

        static int[] _Sort_GetIdx(float[] arr, E_SortDir dir)
        {
            bool isChanged = false;
            int[] arr_idx = new int[arr.Length];
            for (int i = 0; i < arr_idx.Length; i++)
            {
                arr_idx[i] = i;
            }

            if (dir == E_SortDir.Up)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] > arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;

                            var changeidx = arr_idx[j];
                            arr_idx[j] = arr_idx[j + 1];
                            arr_idx[j + 1] = changeidx;

                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            if (dir == E_SortDir.Down)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] < arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;

                            var changeidx = arr_idx[j];
                            arr_idx[j] = arr_idx[j + 1];
                            arr_idx[j + 1] = changeidx;

                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            return arr_idx;
        }
        static int[] _Sort_GetIdx(int[] arr, E_SortDir dir)
        {
            bool isChanged = false;
            int[] arr_idx = new int[arr.Length];
            for (int i = 0; i < arr_idx.Length; i++)
            {
                arr_idx[i] = i;
            }

            if (dir == E_SortDir.Up)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] > arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;

                            var changeidx = arr_idx[j];
                            arr_idx[j] = arr_idx[j + 1];
                            arr_idx[j + 1] = changeidx;

                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            if (dir == E_SortDir.Down)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = 0; j < arr.Length - i - 1; j++)
                    {
                        if (arr[j] < arr[j + 1])
                        {
                            var changevalue = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = changevalue;

                            var changeidx = arr_idx[j];
                            arr_idx[j] = arr_idx[j + 1];
                            arr_idx[j + 1] = changeidx;

                            isChanged = true;
                        }
                        if (j == arr.Length - i - 1 && isChanged == false)
                        {
                            i = arr.Length;
                            break;
                        }
                    }
                }
            }
            return arr_idx;
        }
        #endregion
        public static Collider[] Overlap_Sector(Vector2 pos, float radius, int layermask, float dir, float angleRange, float minDis = 0)
        {
            List<Collider> l_col = Physics.OverlapSphere(pos, radius, layermask).ToList();

            Debug.Log(layermask);
            List<SphereCollider> l_Spherecol = new List<SphereCollider>();
            for (int i = 0; i < l_col.Count; i++)
            {
                SphereCollider cur_sphere;
                if (l_col[i].TryGetComponent<SphereCollider>(out cur_sphere) == false)
                {
                    throw new Exception($"{l_col[i].transform.name} does not have SphereCollider");
                }
                l_Spherecol.Add(cur_sphere);
            }

            //최소사거리 제외
            if (minDis > 0)
            {
                for (int i = 0; i < l_Spherecol.Count; i++)
                {
                    Vector2 target_pos = l_Spherecol[i].transform.position;
                    float dis = Vector2.Distance(pos, target_pos);

                    if (Mathf.Pow((dis + l_Spherecol[i].radius), 2) < minDis * minDis)
                    {
                        l_col.RemoveAt(i);
                        l_Spherecol.RemoveAt(i);
                        i--;
                    }
                }
            }

            //각도확인
            if (angleRange < 360)
            {
                for (int i = 0; i < l_Spherecol.Count; i++)
                {
                    Vector2 obj_pos = l_Spherecol[i].transform.position;
                    float angle = (obj_pos - pos).Angle();
                    float col_radius = l_Spherecol[i].radius;

                    //오브젝트 중심과의 각도 계산
                    if (isAngleRange(dir, angleRange, angle) == false)
                    {
                        bool isHit = false;
                        for (int k = 0; k < 2; k++)
                        {
                            Vector2 gradi;
                            if (k == 0)
                            {
                                gradi = (dir + angleRange * 0.5f).AngleToVt2();
                            }
                            else
                            {
                                gradi = (dir - angleRange * 0.5f).AngleToVt2();
                            }
                            Vector2[] ContactPos = null;
                            Vector2 centerpos;
                            float distance = 0;

                            if (gradi.x == 0)
                            {
                                var data = Get_Distance_Line_Pos_2D(1, 0, -pos.x, obj_pos);
                                centerpos = data.Contact_Pos;
                                distance = data.Distance;
                            }
                            else
                            {
                                var data = Get_Distance_Line_Pos_2D(-gradi.y, gradi.x, gradi.y * pos.x - pos.y * gradi.x, obj_pos);
                                centerpos = data.Contact_Pos;
                                distance = data.Distance;
                            }

                            if (distance < col_radius)
                            {
                                float contact_dis = Mathf.Sqrt(col_radius * col_radius - distance * distance);
                                ContactPos = new Vector2[2] { centerpos + gradi * contact_dis, centerpos - gradi * contact_dis };
                            }
                            else if (distance == col_radius)
                            {
                                ContactPos = new Vector2[1] { centerpos };
                            }
                            else
                            {
                                ContactPos = new Vector2[0];
                            }

                            Vector2 in_point = pos + gradi * minDis;
                            Vector2 side_point = pos + gradi * radius;
                            float min_x = 0;
                            float min_y = 0;
                            float max_x = 0;
                            float max_y = 0;
                            if (in_point.x < side_point.x)
                            {
                                min_x = in_point.x;
                                max_x = side_point.x;
                            }
                            else
                            {
                                min_x = side_point.x;
                                max_x = in_point.x;
                            }
                            if (in_point.y < side_point.y)
                            {
                                min_y = in_point.y;
                                max_y = side_point.y;
                            }
                            else
                            {
                                min_y = side_point.y;
                                max_y = in_point.y;
                            }
                            for (int j = 0; j < ContactPos.Length; j++)
                            {
                                if (ContactPos[j].x >= min_x && ContactPos[j].x <= max_x && ContactPos[j].y >= min_y && ContactPos[j].y <= max_y)
                                {
                                    isHit = true;
                                    break;
                                }
                            }
                        }

                        if (isHit == false)
                        {
                            l_col.RemoveAt(i);
                            l_Spherecol.RemoveAt(i);
                            i--;
                        }
                        //접점이 
                    }
                }
            }

            return l_col.ToArray();
        }

        static int _Get_RandomResult(float[] per)
        {
            float total_value = 0;
            for (int i = 0; i < per.Length; i++)
            {
                if (per[i] > 0)
                {
                    total_value += per[i];
                }
            }

            float random_value = UnityEngine.Random.Range(0, total_value);
            for (int i = 0; i < per.Length; i++)
            {
                random_value -= per[i];
                if (random_value < 0)
                {
                    return i;
                }
            }
            throw new Exception("RandomResult Error : " + total_value + " " + random_value + " " + per.Length);
        }
        public static int Get_RandomResult(float[] per)
        {
            return _Get_RandomResult(per);
        }
        public static int Get_RandomResult(List<float> per)
        {
            return _Get_RandomResult(per.ToArray());
        }

        public static float Get_TimeValue(float curtime, float maxtime, float startvalue = 0, float endvalue = 1)
        {
            if (maxtime == 0)
            {
                return startvalue;
            }
            return startvalue + (curtime / maxtime * (endvalue - startvalue));
        }

        const float Gravity = 9.81f;
        public static Vector2 Get_ParabolaVelocity(float velopower, Vector2 dis, out float time)
        {
            float vx = 0, vy = 0;
            float a = Gravity * Gravity / 4;
            float b = -velopower * velopower + dis.y * Gravity;
            float c = dis.y * dis.y + dis.x * dis.x;

            float center = b * b - 4 * a * c;

            float min_r = Mathf.Sqrt(dis.y * Gravity + Gravity * Mathf.Sqrt(dis.y * dis.y + dis.x * dis.x));
            if (center < 0)
            {
                throw new Exception("최소값 : " + min_r + " 현재값 : " + velopower);
            }

            time = Mathf.Sqrt((-b - Mathf.Sqrt(center)) / (2 * a));
            vx = dis.x / time;
            vy = dis.y / time + Gravity * time / 2;

            return new Vector2(vx, vy + Gravity * Time.fixedDeltaTime * 0.5f);
        }
    }

}