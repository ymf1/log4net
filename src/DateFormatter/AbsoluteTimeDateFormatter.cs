#region Copyright
//
// This framework is based on log4j see http://jakarta.apache.org/log4j
// Copyright (C) The Apache Software Foundation. All rights reserved.
//
// This software is published under the terms of the Apache Software
// License version 1.1, a copy of which has been included with this
// distribution in the LICENSE.txt file.
// 
#endregion

using System;
using System.IO;
using System.Text;

namespace log4net.DateFormatter
{
	/// <summary>
	/// Formats a <see cref="DateTime"/> in the format "HH:mm:ss,SSS" for example, "15:49:37,459".
	/// </summary>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public class AbsoluteTimeDateFormatter : IDateFormatter
	{
		#region Protected Instance Methods

		/// <summary>
		/// Renders the date into a string. Format is "HH:mm:ss".
		/// </summary>
		/// <remarks>
		/// Sub classes should override this method to render the date
		/// into a string using a precision up to the second. This method
		/// will be called at most once per second and the result will be
		/// reused if it is needed again during the same second.
		/// </remarks>
		/// <param name="dateToFormat">The date to render into a string.</param>
		/// <param name="buffer">The string builder to write to.</param>
		virtual protected void FormatDateWithoutMillis(DateTime dateToFormat, StringBuilder buffer)
		{
			int hour = dateToFormat.Hour;
			if (hour < 10) 
			{
				buffer.Append('0');
			}
			buffer.Append(hour);
			buffer.Append(':');

			int mins = dateToFormat.Minute;
			if (mins < 10) 
			{
				buffer.Append('0');
			}
			buffer.Append(mins);
			buffer.Append(':');
	
			int secs = dateToFormat.Second;
			if (secs < 10) 
			{
				buffer.Append('0');
			}
			buffer.Append(secs);
		}

		#endregion Protected Instance Methods

		#region Implementation of IDateFormatter

		/// <summary>
		/// Renders the date into a string. Format is "HH:mm:ss,SSS".
		/// </summary>
		/// <remarks>
		/// <para>Uses the FormatDateWithoutMillis() method to generate the
		/// time string up to the seconds and then appends the current
		/// milliseconds. The results from FormatDateWithoutMillis() are
		/// cached and FormatDateWithoutMillis() is called at most once
		/// per second.</para>
		/// <para>Sub classes should override FormatDateWithoutMillis()
		/// rather than FormatDate().</para>
		/// </remarks>
		/// <param name="dateToFormat">The date to render into a string.</param>
		/// <param name="writer">The writer to write to.</param>
		virtual public void FormatDate(DateTime dateToFormat, TextWriter writer)
		{
			// Calculate the current time precise only to the second
			long currentTimeToTheSecond = (dateToFormat.Ticks - (dateToFormat.Ticks % TimeSpan.TicksPerSecond));

			// Compare this time with the stored last time
			// If we are in the same second then append
			// the previously calculated time string
			if (s_lastTimeToTheSecond != currentTimeToTheSecond)
			{
				// We are in a new second.
				s_lastTimeToTheSecond = currentTimeToTheSecond;
				s_lastTimeBuf.Length = 0;

				// Calculate the new string for this second
				FormatDateWithoutMillis(dateToFormat, s_lastTimeBuf);

				// Store the time as a string (we only have to do this once per second)
				s_lastTimeString = s_lastTimeBuf.ToString();
			}
			writer.Write(s_lastTimeString);
	
			// Append the current milli info
			writer.Write(',');
			int millis = dateToFormat.Millisecond;
			if (millis < 100) 
			{
				writer.Write('0');
			}
			if (millis < 10) 
			{
				writer.Write('0');
			}
			writer.Write(millis);
		}

		#endregion Implementation of IDateFormatter

		#region Public Static Fields

		/// <summary>
		/// String constant used to specify AbsoluteTimeDateFormat in layouts. Current value is <b>ABSOLUTE</b>.
		/// </summary>
		public const string AbsoluteTimeDateFormat = "ABSOLUTE";

		/// <summary>
		/// String constant used to specify DateTimeDateFormat in layouts.  Current value is <b>DATE</b>.
		/// </summary>
		public const string DateAndTimeDateFormat = "DATE";

		/// <summary>
		/// String constant used to specify ISO8601DateFormat in layouts. Current value is <b>ISO8601</b>.
		/// </summary>
		public const string Isi8601TimeDateFormat = "ISO8601";

		#endregion Public Static Fields

		#region Private Static Fields

		/// <summary>
		/// Last stored time with precision up to the second.
		/// </summary>
		private static long s_lastTimeToTheSecond = 0;

		/// <summary>
		/// Last stored time with precision up to the second, formatted
		/// as a string.
		/// </summary>
		private static StringBuilder s_lastTimeBuf = new StringBuilder();

		/// <summary>
		/// Last stored time with precision up to the second, formatted
		/// as a string.
		/// </summary>
		private static string s_lastTimeString;

		#endregion Private Static Fields
	}
}