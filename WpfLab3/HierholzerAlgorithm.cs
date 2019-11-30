using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLab3
{
    public class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
    }

    public class HierholzerAlgorithm
    {
        private Dictionary<int, string> dict = new Dictionary<int, string>();
        private List<List<int>> adj = new List<List<int>>();
        private Dictionary<int, Pair<int, int>> adj_dictionary =
            new Dictionary<int, Pair<int, int>>();

        private int startVertex;

        public HierholzerAlgorithm() { }

        public HierholzerAlgorithm(List<KeyValuePair<string, string>> adj_vertexes)
        {
            foreach (var v in adj_vertexes)
            {
                string v1 = v.Key;
                string v2 = v.Value;

                addVertex(v1);
                addVertex(v2);

                addEdge(v1, v2);
            }

            Console.WriteLine("dictionary:");
            foreach (KeyValuePair<int, string> kvp in dict)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value);
            }
            Console.WriteLine("#####################################");
            Console.WriteLine("adj list:");

            for (int i = 0; i < adj.Count; i++)
            {
                Console.WriteLine(i + ":");
                foreach (var j in adj[i])
                {
                    Console.Write(j + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("#####################################");
            Console.WriteLine("adj_dictionary:");
            foreach (var kvp in adj_dictionary)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value.First + "," + kvp.Value.First);
            }
        }

        public void setEdges(List<KeyValuePair<string, string>> adj_vertexes)
        {
            foreach (var v in adj_vertexes)
            {
                string v1 = v.Key;
                string v2 = v.Value;

                addVertex(v1);
                addVertex(v2);

                addEdge(v1, v2);
            }
        }

        public void setStartEdge(string v1,string v2 )
        {
            int key = dict.FirstOrDefault(x => x.Value==v2).Key;
            startVertex = key;
        }

        //добавить вершину в словарь, если такая уже есть - вернет -1
        //если нет - вернет индекс этой вершины
        private int add(string value)
        {
            int c = dict.Count;
            if (!dict.ContainsValue(value) && !dict.ContainsKey(c))
            {
                Console.WriteLine("Добавляется " + c + value);
                dict.Add(c, value);
                Console.WriteLine("Имеем " + dict[c]);
                return c;
            };
            return -1;
        }

        /*
         Обвноляет список смежных вершин для вершины
         Если вершина уже есть в словаре то не добавить
         увеличить степень смежных в любом случае
         
            true - входящая вершина
            false - выходящая
        */

        private bool updateAdj_forVertex(int key, bool in_out, int i = 1)
        {
            if (adj_dictionary.ContainsKey(key))
            {
                if (in_out) adj_dictionary[key].First += i;
                if (!in_out) adj_dictionary[key].Second += i;
                return true;
            }
            return false;
        }

        private void updateAdjList(int key)
        {
            if (!adj_dictionary.ContainsKey(key)) adj_dictionary.Add(key, new Pair<int, int>());
        }

        //добавляет вершину в список смежностей
        private void addVertex_adjList()
        {
            adj.Add(new List<int>());
        }

        public bool addVertex(string vertex)
        {
            //addVertex(string vt) когда добавляем вершину - добавляем ее в словарь и в список
            //если в словарь добавить удалось(т.е вершина новая - добавить)
            int vertex_idx = add(vertex);
            if (vertex_idx > -1)
            {
                addVertex_adjList();
                return true;
            }
            else
            {
                Console.WriteLine("vertex arl contains " + vertex + ": not added.");
                return false;
            }
        }

        //addEdge(string vertex1 , string vertex2) когда добавляем ребро между вершинами 
        // найти по значению в словаре ключ первой и второй вершины 
        // adj[keyv1].Add(keyv2)
        private void addEdge(string vt1, string vt2)
        {

            int vt1_key = dict.FirstOrDefault(x => x.Value == vt1).Key;
            int vt2_key = dict.FirstOrDefault(x => x.Value == vt2).Key;

            adj[vt1_key].Add(vt2_key);

            updateAdjList(vt1_key);
            updateAdjList(vt2_key);
            updateAdj_forVertex(vt1_key, false);
            updateAdj_forVertex(vt2_key, true);

        }

        // возвращает список ключей для нечетных вершин в словаре
        private List<int> getOddVertexes()
        {
            List<int> odd_vertxes = new List<int>();

            foreach (KeyValuePair<int, Pair<int, int>> pair in adj_dictionary)
            {
                int k = pair.Value.First + pair.Value.Second;
                if (k % 2 != 0) odd_vertxes.Add(pair.Key);
            }
            return odd_vertxes;
        }

        /*
         1)Путь эйлера может быть найден: нечетных вершин две
         */

        /*
         0 - проверка не пройдена
         1 - проверка пройдена полностью
         */
        int christofindes(Pair<int, int> v1, Pair<int, int> v2)
        {
            if (v1.First == v1.Second + 1 && v2.First == v2.Second - 1) { return 1; }
            if (v2.First == v2.Second + 1 && v1.First == v1.Second - 1) { return 0; }

            return -1;
        }


        private bool isEulerPath()
        {
            if(startVertex != 0) { return true; }
            List<int> odd_vertxes = getOddVertexes();
            if (odd_vertxes.Count == 0) return true;
            if (odd_vertxes.Count != 2) return false;

            int c = christofindes(adj_dictionary[odd_vertxes[0]],
            adj_dictionary[odd_vertxes[1]]);

            if (c == 1 || c == 0) { startVertex = odd_vertxes[c]; return true; };

            return false;
        }



        public List<KeyValuePair<string, string>> find_path()
        {
            List<KeyValuePair<string, string>> result_path = new List<KeyValuePair<string, string>>();
            Dictionary<int, int> edge_count = new Dictionary<int, int>();

            for (int i = 0; i < adj.Count(); i++)
            {
                int k = adj[i].Count();
                edge_count[i] = k;
            }

            if (adj.Count() == 0)
                return result_path;


            Stack<int> curr_path = new Stack<int>();
            List<int> circuit = new List<int>();



            if (!isEulerPath())
            {
                Console.WriteLine("Euler Path not exit!");
                return result_path;
            }

            int curr_v = startVertex;
            curr_path.Push(startVertex);
            Console.WriteLine("start vertex is: " + startVertex);

            while (curr_path.Any())
            {
                if (edge_count[curr_v] > 0)
                {
                    curr_path.Push(curr_v);
                    int next_v = adj[curr_v][adj[curr_v].Count - 1];

                    edge_count[curr_v]--;
                    adj[curr_v].RemoveAt(adj[curr_v].Count - 1);

                    curr_v = next_v;
                }

                else
                {
                    circuit.Add(curr_v);
                    curr_v = curr_path.Peek();
                    curr_path.Pop();
                }
            }



            for (int i = circuit.Count() - 1; i > 0; i--)
            {
                string value_v1;
                string value_v2;
                if (
                    dict.TryGetValue(circuit[i], out value_v1) &&
                    dict.TryGetValue(circuit[i - 1], out value_v2)
                    )
                {
                    result_path.Add(new KeyValuePair<string, string>(value_v1, value_v2));
                }
            }

            return result_path;

        }
    }
}
