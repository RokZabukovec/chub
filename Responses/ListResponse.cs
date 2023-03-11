using System.Collections.Generic;
using chub.Models;

namespace chub.Responses;

public class ListResponse<T>
{
    public List<T> Data { get; set; }
}