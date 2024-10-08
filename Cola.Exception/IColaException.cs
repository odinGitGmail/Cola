﻿namespace Cola.Exception;

public interface IColaException
{
    /// <summary>
    /// throw number>0
    /// </summary>
    /// <param name="i"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    System.Exception? ThrowGreaterThanZero(int i, string errorMessage);
    
    /// <summary>
    /// object is null
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    System.Exception? ThrowIfNull<T>(T obj);
    
    /// <summary>
    /// throw String Is NullOrEmpty
    /// </summary>
    /// <param name="str"></param>
    /// <param name="exMessage"></param>
    /// <returns></returns>
    System.Exception? ThrowStringIsNullOrEmpty(string str, string exMessage);
    
    /// <summary>
    /// Throw Exception
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    System.Exception ThrowException(string str);
    
    /// <summary>
    /// Throw Exception
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    System.Exception ThrowException(System.Exception ex);
}