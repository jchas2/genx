<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:md="http://genx.com/metadata" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" encoding="UTF-8" indent="yes" />
<xsl:preserve-space elements="*"/>

<!-- Mandatory parameter from the genx cli -->
<xsl:param name="EntityName" />

<!-- Custom parameters -->
<xsl:param name="OrganisationName" />
<xsl:param name="ApplicationName" />
<xsl:param name="ServiceName" />

<xsl:template match="/">
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:entitycolumns" />
</xsl:template>

<xsl:template match="md:entitycolumns">
<xsl:variable name="EntityName" select="ancestor::md:entity/@name"/>
<xsl:variable name="EntityNameVar" select="ancestor::md:entity/@camelcase"/>
<xsl:variable name="EntityOriginalName" select="ancestor::md:entity/@originalname"/>
using <xsl:value-of select="$OrganisationName"/>.<xsl:value-of select="$ServiceName"/>.Core.Domain;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

// TODO: Verify namespace.
namespace <xsl:value-of select="$ApplicationName"/>.<xsl:value-of select="$ServiceName"/>.Core.Command
{
    public class Update<xsl:value-of select="$EntityName"/>CommandValidator : AbstractValidator&lt;Update<xsl:value-of select="$EntityName"/>Command&gt;
    {
		<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="member_variables" />	
		private readonly I<xsl:value-of select="$EntityName"/>Repository _<xsl:value-of select="$EntityNameVar"/>Repository;

		public Update<xsl:value-of select="$EntityName"/>CommandValidator(
    		<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="constructor" />	
		    I<xsl:value-of select="$EntityName"/>Repository <xsl:value-of select="$EntityNameVar"/>Repository)
		{
    		<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="constructor_assignment" />
			_<xsl:value-of select="$EntityNameVar"/>Repository = <xsl:value-of select="$EntityNameVar"/>Repository ?? throw new ArgumentNullException(nameof(<xsl:value-of select="$EntityNameVar"/>Repository));

<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='true'">
            RuleFor(command => command.<xsl:value-of select="@name"/>)
			    .NotEmpty()
				.MustAsync(HasValid<xsl:value-of select="@name"/>Async)
				.WithMessage(command => string.Format(LocalisableStrings.<xsl:value-of select="$EntityName"/><xsl:value-of select="@name"/>Error, command.<xsl:value-of select="@name"/>);
                				
</xsl:if></xsl:for-each>

    		<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="validators" />	

<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='false'">
            RuleFor(command => command.<xsl:value-of select="@name"/>)
			    <xsl:if test="@allownulls!='true'">.NotEmpty()</xsl:if>
				<xsl:if test="@datatype='char' or @datatype='nchar' or @datatype='nvarchar' or @datatype='varchar'">
				.MaximumLength(<xsl:value-of select="@maxlength"/>)
				.WithMessage(command => string.Format(LocalisableStrings.<xsl:value-of select="@name"/>LengthExceeded, <xsl:value-of select="@maxlength"/>)
                .WithCustomText())</xsl:if>
				;
				</xsl:if></xsl:for-each>

<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='false'">
</xsl:if></xsl:for-each>
		}
<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='true'">
		private async Task&lt;bool&gt; HasValid<xsl:value-of select="@name"/>Async(<xsl:call-template name="sqltocsharp"><xsl:with-param name="entitycolumn" select="."/></xsl:call-template><xsl:text> </xsl:text><xsl:value-of select="@camelcase"/>, CancellationToken cancellationToken)
		{
		    return await _<xsl:value-of select="$EntityNameVar"/>Repository.GetAsync(<xsl:value-of select="@camelcase"/>) != null;
        }       				
</xsl:if></xsl:for-each>
	}
}		
</xsl:template>

<xsl:template match="md:relationships" mode="member_variables">
<xsl:for-each select="md:relationship">private readonly I<xsl:value-of select="@foreignkeyentity"/>Repository _<xsl:value-of select="@foreignkeyentitycamelcase"/>Repository;
		</xsl:for-each>
</xsl:template>

<xsl:template match="md:relationships" mode="constructor">
<xsl:for-each select="md:relationship">I<xsl:value-of select="@foreignkeyentity"/>Repository <xsl:value-of select="@foreignkeyentitycamelcase"/>Repository,
			</xsl:for-each>
</xsl:template>

<xsl:template match="md:relationships" mode="constructor_assignment">
<xsl:for-each select="md:relationship">_<xsl:value-of select="@foreignkeyentitycamelcase"/>Repository = <xsl:value-of select="@foreignkeyentitycamelcase"/>Repository ?? throw new ArgumentNullException(nameof(<xsl:value-of select="@foreignkeyentityCamelCase"/>Repository));
			</xsl:for-each>
</xsl:template>

<xsl:template match="md:relationships" mode="validators">
<xsl:for-each select="md:relationship">
            // TODO: Verify validator.
            RuleForEach(command => command.<xsl:value-of select="@foreignkeyentity"/>)
			    SetValidator(new <xsl:value-of select="@foreignkeyentity"/>DtoValidator(<xsl:value-of select="@foreignkeyentitycamelcase"/>Repository));
		</xsl:for-each>
</xsl:template>

<xsl:template name="sqltocsharp">
	<xsl:param name="entitycolumn"/>
	<xsl:choose>
		<xsl:when test="@datatype='bigint'">long</xsl:when>
		<xsl:when test="@datatype='binary'">byte[]</xsl:when>
		<xsl:when test="@datatype='bit'">bool</xsl:when>
		<xsl:when test="@datatype='char'">string</xsl:when>
		<xsl:when test="@datatype='datetime'">DateTime</xsl:when>
		<xsl:when test="@datatype='decimal'">decimal</xsl:when>
		<xsl:when test="@datatype='float'">double</xsl:when>
		<xsl:when test="@datatype='int'">int</xsl:when>
		<xsl:when test="@datatype='image'">byte[]</xsl:when>
		<xsl:when test="@datatype='money'">decimal</xsl:when>
		<xsl:when test="@datatype='nchar'">string</xsl:when>
		<xsl:when test="@datatype='numeric'">decimal</xsl:when>
		<xsl:when test="@datatype='ntext'">string</xsl:when>
		<xsl:when test="@datatype='nvarchar'">string</xsl:when>
		<xsl:when test="@datatype='real'">Single</xsl:when>
		<xsl:when test="@datatype='smallint'">short</xsl:when>
		<xsl:when test="@datatype='text'">string</xsl:when>
		<xsl:when test="@datatype='tinyint'">byte</xsl:when>
		<xsl:when test="@datatype='uniqueidentifier'">Guid</xsl:when>		
		<xsl:when test="@datatype='varbinary'">byte[]</xsl:when>		
		<xsl:when test="@datatype='varchar'">string</xsl:when>		
	</xsl:choose>
</xsl:template>

</xsl:stylesheet> 
