﻿using System;
using System.Collections.Generic;

namespace PhoneSkill.Models
{
    /// <summary>
    /// The result of a search for a contact in the user's contact list.
    /// </summary>
    public class ContactSearchResult : IEquatable<ContactSearchResult>
    {
        /// <summary>
        /// Gets or sets the search query that led to these results.
        /// </summary>
        /// <value>
        /// The search query that led to these results. This is typically an entity value from the LUIS result.
        /// </value>
        public string SearchQuery { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of phone number that the user requested.
        /// </summary>
        /// <value>
        /// The type of phone number that the user requested.
        /// </value>
        public PhoneNumberType RequestedPhoneNumberType { get; set; } = new PhoneNumberType();

        /// <summary>
        /// Gets or sets the matching contacts.
        /// </summary>
        /// <value>
        /// The matching contacts, sorted from best to worst match.
        /// If there are multiple matches, the user will be asked to choose one and this list will be updated with their choice.
        /// </value>
        public IList<ContactMatch> Matches { get; set; } = new List<ContactMatch>();

        public override bool Equals(object obj)
        {
            return Equals(obj as ContactSearchResult);
        }

        public bool Equals(ContactSearchResult other)
        {
            return other != null &&
                   SearchQuery == other.SearchQuery &&
                   EqualityComparer<PhoneNumberType>.Default.Equals(RequestedPhoneNumberType, other.RequestedPhoneNumberType) &&
                   EqualityComparer<IList<ContactMatch>>.Default.Equals(Matches, other.Matches);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SearchQuery, RequestedPhoneNumberType, Matches);
        }

        public override string ToString()
        {
            return $"ContactSearchResult{{SearchQuery={SearchQuery}, RequestedPhoneNumberType={RequestedPhoneNumberType}, Matches={Matches}}}";
        }
    }
}