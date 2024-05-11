using System.Collections.Generic;

namespace chub.Responses;

public class ListResponse<T>
{
    public List<T> Data { get; set; }
}