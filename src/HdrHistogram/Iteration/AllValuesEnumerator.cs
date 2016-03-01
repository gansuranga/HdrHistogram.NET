﻿/*
 * Written by Matt Warren, and released to the public domain,
 * as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 *
 * This is a .NET port of the original Java version, which was written by
 * Gil Tene as described in
 * https://github.com/HdrHistogram/HdrHistogram
 */

using System.Collections;
using System.Collections.Generic;

namespace HdrHistogram.Iteration
{
    /// <summary>
    /// An enumerator of <see cref="HistogramIterationValue"/> through the histogram using a <see cref="AllValuesEnumerator"/>
    /// </summary>
    internal sealed class AllValueEnumerable : IEnumerable<HistogramIterationValue>
    {
        private readonly HistogramBase _histogram;

        public AllValueEnumerable(HistogramBase histogram)
        {
            _histogram = histogram;
        }

        public IEnumerator<HistogramIterationValue> GetEnumerator()
        {
            return new AllValuesEnumerator(_histogram);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// Used for iterating through histogram values using the finest granularity steps supported by the underlying
    /// representation.The iteration steps through all possible unit value levels, regardless of whether or not
    /// there were recorded values for that value level, and terminates when all recorded histogram values are exhausted.
    /// </summary>
    internal sealed class AllValuesEnumerator : AbstractHistogramEnumerator
    {
        private int _visitedSubBucketIndex;
        private int _visitedBucketIndex;

        /// <summary>
        /// Constructor for the <see cref="AllValuesEnumerator"/>.
        /// </summary>
        /// <param name="histogram">The histogram this iterator will operate on</param>
        public AllValuesEnumerator(HistogramBase histogram):base(histogram)
        {
            _visitedSubBucketIndex = -1;
            _visitedBucketIndex = -1;
        }

        protected override void IncrementIterationLevel()
        {
            _visitedSubBucketIndex = CurrentSubBucketIndex;
            _visitedBucketIndex = CurrentBucketIndex;
        }

        protected override bool ReachedIterationLevel()
        {
            return (_visitedSubBucketIndex != CurrentSubBucketIndex) 
                || (_visitedBucketIndex != CurrentBucketIndex);
        }
    }
}
