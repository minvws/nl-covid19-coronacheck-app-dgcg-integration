# Plannig for V0.8

* This release will provide support for download of revocation lists and upload of revocation batches.

# V0.7

* Removed support for the CMS signed wrapper, deprecating the `-w` flag and related certificate configuration.
  * Section `CmsSignature` removed.
  * Section `CmsSignatureChain` removed.
* CLI migrated to use git-like verbs, in preparation for release 0.8:
  * Download is now `dggt download <args>` instead of `dggt -d`.
  * Upload is now `dggt download <args>` instead of `dggt -u`.
  * Revocation is now `dggt download <args>` instead of `dggt -u`.
