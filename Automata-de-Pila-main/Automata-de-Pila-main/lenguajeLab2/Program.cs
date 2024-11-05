using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    public class Node
    {
        public string Value;
        public List<Node> Children;

        public Node(string value)
        {
            Value = value;
            Children = new List<Node>();
        }
    }

    static void Main(string[] args)
    {
        //La ruta del archivo con las cadenas que se leerán, deben estar guardadas en: "Automata-de-Pila-main\Automata-de-Pila-main\lenguajeLab2\bin\Debug\net8.0"
        string filePath = "cadenasAceptadas.txt";

        try
        {
            // Leer todas las cadenas del archivo de texto
            string[] cadenas = File.ReadAllLines(filePath);

            // Tabla de transiciones general
            Dictionary<string, Dictionary<char, string>> tablaTransicionesGeneral = new Dictionary<string, Dictionary<char, string>>();

            // Procesar las primeras 50 cadenas
            for (int i = 0; i < 30; i++)
            {
                string cadena = cadenas[i].Trim();
                var soloAsteriscosYNumerales = new string(cadena.Where(c => c == '*' || c == '#').ToArray());

                Console.WriteLine($"\nProcesando cadena {i + 1}: '{cadena}'\n");

                // Inicializar el árbol de derivación
                Node root = new Node("q0");

                Console.WriteLine("SIMBOLOGÍA:");
                Console.WriteLine($"L: {cadena}");
                Console.WriteLine($"Caracteres especiales: {soloAsteriscosYNumerales}");
                Console.WriteLine($"Estados: q1, q2, q3, q4");
                Console.WriteLine($"Estado final: q4");
                Console.WriteLine($"Transiciones: I, D");

                // Verificar si la cadena es válida y generar las transiciones
                bool esValida = VerificarCadena(cadena, root, tablaTransicionesGeneral);

                // Mostrar el árbol de derivación
                Console.WriteLine("\nGRAFO:");
                PrintTree(root, "", true);

                // Inicializar dos arrays: uno para caracteres en posiciones impares y otro para posiciones pares
                List<char> charsImpares = new List<char>();
                List<char> charsPares = new List<char>();

                // Separar caracteres de la cadena
                for (int j = 0; j < cadena.Length; j++)
                {
                    if (j % 2 == 0)
                    {
                        // Índice impar (recordando que los arrays inician en 1)
                        charsImpares.Add(cadena[j]);
                    }
                    else
                    {
                        // Índice par
                        charsPares.Add(cadena[j]);
                    }
                }

                // Mostrar el contenido de los arrays
                Console.WriteLine("\nCINTA1:");
                Console.WriteLine(string.Join(", ", charsImpares));

                Console.WriteLine("CINTA2:");
                Console.WriteLine(string.Join(", ", charsPares));

                LeerConDI(charsImpares, charsPares);
                LeerConID(charsImpares, charsPares);

                Console.WriteLine("TABLA DE TRANSICIÓN: ");
                

                // Mostrar la tabla de transición general
                Console.WriteLine("\nTabla de transición general:");
                PrintTransitionTable(tablaTransicionesGeneral);

                // Mostrar el resultado
                if (esValida)
                {
                    Console.WriteLine($"\nLa cadena '{cadena}' es válida según el autómata.");
                }
                else
                {
                    Console.WriteLine($"\nLa cadena '{cadena}' no es válida según el autómata.");
                }

                Console.WriteLine(new string('-', 30));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }
    }

    static bool VerificarCadena(string cadena, Node root, Dictionary<string, Dictionary<char, string>> tablaTransicionesGeneral)
{
    string estadoActual = "q0";  // Estado inicial
    Node currentNode = root;
    bool esValida = false;  // Variable para verificar si la cadena es válida

    foreach (char caracter in cadena)
    {
        string estadoSiguiente = null;

        switch (estadoActual)
        {
            case "q0":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q1";
                }
                break;

            case "q1":
                if (caracter == 'b')
                {
                    estadoSiguiente = "q2";
                }
                break;

            case "q2":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q3";
                }
                break;

            case "q3":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q3";
                }
                else if (caracter == 'b')
                {
                    estadoSiguiente = "q1";
                }
                else if (caracter == '*')
                {
                    estadoSiguiente = "q4";
                }
                break;

            case "q4":
                if (caracter == 'a')
                {
                    estadoSiguiente = "q3";
                }
                else if (caracter == 'b')
                {
                    estadoSiguiente = "q1";
                }

                else if (caracter == '#')
                {
                    // La cadena es válida si termina en el estado q4 con el carácter '#'
                    esValida = true;
                    estadoSiguiente = "q4";  // Mantener el estado como q4 (estado final)
                    break;
                }
                break;
        }

        // Agregar la transición a la tabla de transiciones general
        if (estadoSiguiente != null)
        {
            if (!tablaTransicionesGeneral.ContainsKey(estadoActual))
            {
                tablaTransicionesGeneral[estadoActual] = new Dictionary<char, string>();
            }

            if (!tablaTransicionesGeneral[estadoActual].ContainsKey(caracter))
            {
                tablaTransicionesGeneral[estadoActual][caracter] = estadoSiguiente;
            }

            // Actualizar el árbol de derivación y las pilas
            Node nextNode = new Node(estadoSiguiente);
            currentNode.Children.Add(nextNode);
            currentNode = nextNode;

            estadoActual = estadoSiguiente;
        }
        else
        {
            Node errorNode = new Node($"Error ({caracter})");
            currentNode.Children.Add(errorNode);
            currentNode = errorNode;
        }
    }

    return esValida;  // Devolver si la cadena fue válida o no
}
    static void PrintTransitionTable(Dictionary<string, Dictionary<char, string>> tablaTransiciones)
    {
        char[] caracteres = { 'a', 'b', '*', '#' };

        Console.WriteLine("      a     b     *     #");
        Console.WriteLine("    ------------------------");

        foreach (var estado in tablaTransiciones)
        {
            Console.Write($"{estado.Key}  ");

            foreach (var caracter in caracteres)
            {
                if (estado.Value.ContainsKey(caracter))
                {
                    Console.Write($"{estado.Value[caracter],-5} ");
                }
                else
                {
                    Console.Write($"{"-",-5} ");
                }
            }

            Console.WriteLine();
        }
    }

    static void PrintTree(Node node, string indent, bool last)
    {
        Console.Write(indent);
        if (last)
        {
            Console.Write("└─");
            indent += "  ";
        }
        else
        {
            Console.Write("├─");
            indent += "| ";
        }
        Console.WriteLine(node.Value);

        for (int i = 0; i < node.Children.Count; i++)
        {
            PrintTree(node.Children[i], indent, i == node.Children.Count - 1);
        }
    }
    static void LeerConDI(List<char> cinta1, List<char> cinta2)
    {
        Console.WriteLine("DI:");
        for (int i = 0; i < cinta1.Count; i++)
        {
            Console.Write(cinta1[i] + " ");
        }
        for (int i = 0; i < cinta2.Count; i++)
        {
            Console.Write(cinta2[i] + " ");
        }
        Console.WriteLine();  // Salto de línea
    }

    // Método para leer cintas de derecha a izquierda
    static void LeerConID(List<char> cinta1, List<char> cinta2)
    {
        Console.WriteLine("ID:");
        for (int i = cinta1.Count - 1; i >= 0; i--)
        {
            Console.Write(cinta1[i] + " ");
        }
        for (int i = cinta2.Count - 1; i >= 0; i--)
        {
            Console.Write(cinta2[i] + " ");
        }
        Console.WriteLine();  // Salto de línea
    }
}