﻿using System;
using System.Collections.Generic;
using System.Linq;


public class RModel<T>
{

    public IQueryable<T> Result { get; set; }
    public List<T> ResultList = new List<T>();
    public DTResult<T> ResultPaging { get; set; }
    public T ResultRow { get; set; }
    public string ResultJson { get; set; }
    public string RedirectUrl { get; set; }
    public string Message { get; set; }
    public Exception Ex { get; set; }
    public RType RType { get; set; }

    public List<string> MessageList = new List<string>();
    public string MessageListJson { get; set; }

    public string Joker { get; set; }
    public string Joker2 { get; set; }

}



public enum RType : int
{
    OK = 1,
    Error = 2,
    Warning = 3
}