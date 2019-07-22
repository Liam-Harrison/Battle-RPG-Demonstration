using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;

    int currentCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentCount;
        items[currentCount] = item;
        SortUp(item);
        currentCount++;
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parent = items[parentIndex];
            if (item.CompareTo(parent) > 0)
            {
                Swap(item, parent);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    public T RemoveFirst()
    {
        T first = items[0];
        currentCount--;
        items[0] = items[currentCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return first;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
       get
        {
            return currentCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T a)
    {
        while (true)
        {
            int childIntLeft = a.HeapIndex * 2 + 1;
            int childIntRight = a.HeapIndex * 2 + 2;
            int swap = 0;

            if (childIntLeft < currentCount)
            {
                swap = childIntLeft;

                if (childIntRight < currentCount)
                {
                    if (items[childIntLeft].CompareTo(items[childIntRight]) < 0)
                    {
                        swap = childIntRight;
                    }
                }

                if (a.CompareTo(items[swap]) < 0)
                {
                    Swap(a, items[swap]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        int aIndex = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = aIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}