namespace AccountLib.Contracts
{
	//public class ResultWithMessage(object data, string message)
	//{
	//	public Object? Data { get; set; } = data;
	//	public string? Message { get; set; } = message;
	//}
	public class ResultWithMessage<T>
	{
		public T? Data { get; set; }
		public string? Message { get; set; }

		public ResultWithMessage(T data, string message)
		{
			Data = data;
			Message = message;
		}
	}
	public class DataWithSize(int dataSize, object data)
	{
		public int DataSize { get; set; } = dataSize;
		public object Data { get; set; } = data;
	}
}
