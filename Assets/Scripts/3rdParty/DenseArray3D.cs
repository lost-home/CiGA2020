using System;
using System.Collections.Generic;

public class DenseArray3D<T> : IEnumerable<T>
{
    private T[] _dataarray;
    private int[] _rowindexes;
    //private int[] _columnindexes;

    /// <summary>
    /// Calculates the row indexes
    /// </summary>
    private void CalculateCacheIndexes()
    {
        _rowindexes = new int[Rows];
        for (int i = 0; i < Rows; i++)
        {
            _rowindexes[i] = i * Columns;
        }

        //_columnindexes = new int[Columns];
        //for (int i = 0; i < Columns; i++)
        //{
        //    _columnindexes[i] = i * Rows;
        //}
    }

    /// <summary>
    /// Creates a new instance of DenseArray
    /// </summary>
    /// <param name="rows">Number of rows</param>
    /// <param name="columns">Number of columns</param>
    public DenseArray3D(int rows, int columns, int depths)
    {
        _dataarray = new T[rows * columns * depths];
        Rows = rows;
        Columns = columns;
        Depths = depths;
        CalculateCacheIndexes();
    }

    /// <summary>
    /// Creates a new instance of DenseArray
    /// </summary>
    /// <param name="source">Source DenseArray</param>
    public DenseArray3D(DenseArray3D<T> source)
    {
        _dataarray = new T[source.Rows * source.Columns * source.Depths];
        Rows = source.Rows;
        Columns = source.Columns;
        Depths = source.Depths;
        Array.Copy(source._dataarray, this._dataarray, source._dataarray.LongLength);
        _rowindexes = new int[Rows];
        Array.Copy(source._rowindexes, this._rowindexes, source._rowindexes.LongLength);
    }

    /// <summary>
    /// Creates a new instance of DenseArray
    /// </summary>
    /// <param name="array">source 2d array</param>
    public DenseArray3D(T[,,] array)
    {
        Rows = array.GetLength(0);
        Columns = array.GetLength(1);
        Depths = array.GetLength(2);
        _dataarray = new T[Rows * Columns * Depths];
        CalculateCacheIndexes();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                for (int k = 0; k < Depths; k++)
                {
                    this[i, j, k] = array[i, j, k];
                }
            }
        }
    }

    /// <summary>
    /// Gets the number of columns
    /// </summary>
    public int Columns { get; private set; }

    /// <summary>
    /// Gets the number of rows
    /// </summary>
    public int Rows { get; private set; }

    /// <summary>
    /// Gets the number of depths
    /// </summary>
    public int Depths { get; private set; }

    /// <summary>
    /// Gets the size of the DenseArray
    /// </summary>
    public int Length => Rows * Columns * Depths;

    /// <summary>
    /// Gets or sets an element of the array
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <returns>Value at row and column index</returns>
    public T this[int row, int column, int depth]
    {
        get { return _dataarray[depth + Depths * (column + _rowindexes[row])]; }
        set { _dataarray[depth + Depths * (column + _rowindexes[row])] = value; }
    }

    /// <summary>
    /// Gets or sets an element of the array
    /// </summary>
    /// <param name="i">Index</param>
    /// <returns>Value at index</returns>
    public T this[int i]
    {
        get { return _dataarray[i]; }
        set { _dataarray[i] = value; }
    }


    /// <summary>
    /// IEnumerable implementation.
    /// </summary>
    /// <returns>internal array enumerator</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return (IEnumerator<T>)_dataarray.GetEnumerator();
    }

    /// <summary>
    /// IEnumerable Implementation
    /// </summary>
    /// <returns>internal array enumerator</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _dataarray.GetEnumerator();
    }
}
